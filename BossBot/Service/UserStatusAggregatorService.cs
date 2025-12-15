using BossBot.Model;
using BossBot.Options;
using CommonLib.Requests;
using Discord;
using Discord.WebSocket;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BossBot.Service;

public class UserStatusAggregatorService
{
    private const string SheetName = "General";

    private const int DataStartRow = 2;
    private const string LastColumn = "AA";

    private readonly DiscordClientService _discordClientService;
    private readonly SheetsService _sheetsService;
    private readonly OpenAIService _openAiService;
    private readonly BotOptions _options;

    public UserStatusAggregatorService(DiscordClientService discordClientService, OpenAIService openAiService, BotOptions options)
    {
        _discordClientService = discordClientService;
        _openAiService = openAiService;
        _options = options;
        var credential = GoogleCredential
            .FromFile("GoogleKeys.json")
            .CreateScoped(SheetsService.Scope.Spreadsheets);

        _sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "DiscordBossBot"
        });
        _discordClientService.MessageReceivedEvent += DiscordMessageReceivedEvent;
    }

    private void DiscordMessageReceivedEvent(object? sender, Tuple<IMessage, ISocketMessageChannel> e)
    {
        if(e.Item2 is IDMChannel channel)
        {
            _ = ReadSheet(e.Item2, e.Item1.Attachments.Select(a => a.Url).ToList(), e.Item1.Author.GlobalName ?? e.Item1.Author.Username);
        }
    }

    private async Task ReadSheet(ISocketMessageChannel dbChannel, List<string> attachmentUrls, string discordNick)
    {
        CharacterStatsModel model = null;
        try
        {
            List<string> subStrings = [];
            foreach (var attachmentUrl in attachmentUrls)
            {
                var requestData = new RequestParseImageUrl
                {
                    Url = attachmentUrl,
                };
                var jsonPayload = JsonSerializer.Serialize(requestData);
                using var httpClient = new HttpClient();
                var imageAnalyzeResponse = await httpClient.PostAsync(_options.ImageStatisticAnalysisUrl,
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                var stringResponse = await imageAnalyzeResponse.Content.ReadAsStringAsync();
                subStrings.AddRange(stringResponse.Split(","));
            }

            model = CreateModel(subStrings);

            await MergeUpsertAsync(discordNick, string.Empty, model);
            await _discordClientService.ProcessAnswers(dbChannel, [$"Информация была обновлена {model.ToString()}"]);
        }
        catch(Exception ex)
        {
            await _discordClientService.ProcessAnswers(dbChannel, [$"Информация не была обновлена {model?.ToString() ?? "модель не была создана"}"]);
        }
    }


    public async Task<int> MergeUpsertAsync(
            string discordNick,
            string? inGameNick,
            CharacterStatsModel newStats,
            CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(discordNick))
            throw new ArgumentException("discordNick is required", nameof(discordNick));

        // 1) Find row by Discord nick (Column A)
        var (found, rowNumber) = await FindRowByDiscordNickAsync(discordNick, ct);

        if (!found)
        {
            // Not found -> create row (append full row A..AA)
            var newRow = BuildFullRow(discordNick, inGameNick, newStats);

            var appendRange = $"{SheetName}!A:{LastColumn}";
            var appendBody = new ValueRange { Values = new List<IList<object>> { newRow } };

            var appendReq = _sheetsService.Spreadsheets.Values.Append(appendBody, _options.GoogleSheetId, appendRange);
            appendReq.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest
                .ValueInputOptionEnum.USERENTERED;
            appendReq.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest
                .InsertDataOptionEnum.INSERTROWS;

            var appendResp = await appendReq.ExecuteAsync(ct);

            // updatedRange like "Sheet1!A15:AA15"
            return TryParseRowFromA1Range(appendResp.Updates?.UpdatedRange) ?? -1;
        }

        // 2) Read existing row A..AA
        var rowRange = $"{SheetName}!A{rowNumber}:{LastColumn}{rowNumber}";
        var getReq = _sheetsService.Spreadsheets.Values.Get(_options.GoogleSheetId, rowRange);
        var getResp = await getReq.ExecuteAsync(ct);

        var existingRow = (getResp.Values is { Count: > 0 } ? getResp.Values[0] : new List<object>())
            .Select(x => x?.ToString() ?? "")
            .ToList();

        // Ensure we have 27 columns (A..AA)
        while (existingRow.Count < 27) existingRow.Add("");

        // 3) Merge keys
        // A = discordNick (keep existing; or you can force normalize)
        existingRow[0] = discordNick;

        // B = inGameNick: update only if provided (non-empty)
        if (!string.IsNullOrWhiteSpace(inGameNick))
            existingRow[1] = inGameNick!.Trim();

        // 4) Merge stats (C..AA) only where incoming value != null
        var incomingStatsCells = BuildStatsValues(newStats); // 25 cells (C..AA)

        // existingRow index 2 maps to column C
        for (int i = 0; i < incomingStatsCells.Count; i++)
        {
            var incoming = incomingStatsCells[i];
            if (incoming is null) continue; // do not overwrite

            existingRow[2 + i] = incoming.Value.ToString();
        }

        // 5) Update row back
        var updateBody = new ValueRange
        {
            Values = new List<IList<object>> { existingRow.Cast<object>().ToList() }
        };

        var updateReq = _sheetsService.Spreadsheets.Values.Update(updateBody, _options.GoogleSheetId, rowRange);
        updateReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest
            .ValueInputOptionEnum.USERENTERED;

        await updateReq.ExecuteAsync(ct);

        return rowNumber;
    }

    private async Task<(bool found, int rowNumber)> FindRowByDiscordNickAsync(
        string discordNick,
        CancellationToken ct)
    {
        var readRange = $"{SheetName}!A{DataStartRow}:A";
        var req = _sheetsService.Spreadsheets.Values.Get(_options.GoogleSheetId, readRange);
        var resp = await req.ExecuteAsync(ct);

        var values = resp.Values ?? new List<IList<object>>();

        for (int i = 0; i < values.Count; i++)
        {
            var cell = values[i].Count > 0 ? values[i][0]?.ToString() : null;
            if (cell is null) continue;

            if (string.Equals(cell.Trim(), discordNick.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var rowNumber = DataStartRow + i;
                return (true, rowNumber);
            }
        }

        return (false, -1);
    }

    // Full row A..AA (27 cells). For new rows, write "" for nulls.
    private IList<object> BuildFullRow(string discordNick, string? inGameNick, CharacterStatsModel stats)
    {
        var row = new List<object>
        {
            discordNick,
            string.IsNullOrWhiteSpace(inGameNick) ? "" : inGameNick!.Trim()
        };

        // 25 stats -> C..AA
        foreach (var v in BuildStatsValues(stats))
            row.Add(v?.ToString() ?? "");

        // Ensure exactly 27 cells
        while (row.Count < 27) row.Add("");

        return row;
    }

    // Returns 25 nullable ints in EXACT order of your columns C..AA
    private List<int?> BuildStatsValues(CharacterStatsModel s) =>
    [
        s.Damage,
        s.Accuracy,
        s.CritAtkPercent,

        // --- Crit / multi-hit / block ---
        s.BonusCritDamage,
        s.DoubleDamageChancePercent,
        s.TripleDamageChancePercent,
        s.WeaponBlockPercent,

        // --- Defense ---
        s.Defense,
        s.SkillResistance,

        // --- Damage increase / resist ---
        s.WeaponDamageIncreasePercent,
        s.WeaponDamageResistancePercent,
        s.SkillDamageIncreasePercent,
        s.SkillDamageResistancePercent,

        // --- Special resists / penetration / CC ---
        s.DoubleDamageResistancePercent,
        s.TripleDamageResistancePercent,
        s.BlockPenetrationPercent,
        s.IgnoreDamageReduction,
        s.StunChancePercent,

        // --- Control resist / abnormal ---
        s.StunResistancePercent,
        s.HoldResistancePercent,
        s.AggroResistancePercent,
        s.SilenceResistancePercent,
        s.AbnormalStatusChancePercent,
        s.AbnormalStatusResistancePercent,
        s.AbnormalStatusDurationReductionPercent
    ];

    private int? TryParseRowFromA1Range(string? a1)
    {
        // "Sheet1!A15:AA15" -> 15
        if (string.IsNullOrWhiteSpace(a1)) return null;
        var bang = a1.IndexOf('!');
        if (bang >= 0) a1 = a1[(bang + 1)..];

        var digits = new string(a1.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var row) ? row : null;
    }

    private CharacterStatsModel CreateModel(List<string> strings)
    {
        CharacterStatsModel model = new();
        var clearResult = strings.Select(NormalizeString);
        var previous = string.Empty;
        foreach (var s in clearResult)
        {
            if (Int32.TryParse(s, out var value))
            {
                PopulateModel(model, previous, value);
            }
            else
                if (IsValidParameter(s))
                    previous = s;
        }

        return model;
    }

    static string NormalizeString(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return string.Empty;

        var sb = new StringBuilder(s.Length);

        foreach (var c in s)
        {
            // оставляем только буквы и цифры (RU + EN)
            if (char.IsLetterOrDigit(c))
                sb.Append(char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }

    private void PopulateModel(CharacterStatsModel model, string name, int value)
    {
        if (LevenshteinWithin(name, "уронвдальнбою", "rangeddamage", "уронвближнбою", "meleedamage", "магурон", "magicdamage"))
        {
            model.Damage = value;
            return;
        }

        if (LevenshteinWithin(name, "точностьвдальнбою", "rangedaccuracy", "магточность", "magicaccuracy", "точностьвближнбою", "meleeaccuracy"))
        {
            model.Accuracy = value; 
            return;
        }

        if (LevenshteinWithin(name, "критатквдальнбою", "rangedcriticalhit", "магкритатк", "критатквближнбою", "magiccriticalhit", "meleecriticalhit"))
        {
            model.CritAtkPercent = value;
            return;
        }

        if (LevenshteinWithin(name, "допуронкритатк", "extradamageoncriticalhit"))
        {
            model.BonusCritDamage = value;
            return;
        }

        if (LevenshteinWithin(name, "шансдвойногоурона", "doublechance"))
        {
            model.DoubleDamageChancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "шанстройногоурона", "triplechance"))
        {
            model.TripleDamageChancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениедвойномуурону", "doubleresistance"))
        {
            model.DoubleDamageResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениетройномуурону", "tripleresistance"))
        {
            model.TripleDamageResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "блокировкаоружия", "weaponblock"))
        {
            model.WeaponBlockPercent = value;
            return;
        }

        if (LevenshteinWithin(name, "пробиваниеблока", "blockpenetration"))
        {
            model.BlockPenetrationPercent = value;
            return;
        }

        if (LevenshteinWithin(name, "увеличениеуронаоторужия", "weapondamageboost"))
        {
            model.WeaponDamageIncreasePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "увеличениеуронаотумений", "skilldamageboost"))
        {
            model.SkillDamageIncreasePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "защита", "defense"))
        {
            model.Defense = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениеумениям", "skillresistance"))
        {
            model.SkillResistance = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениеуроноторужия", "weapondefense"))
        {
            model.WeaponDamageResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениеуронотумений", "skilldefense"))
        {
            model.SkillDamageResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "игнорированиесниженияурона", "ignoredamagereduction"))
        {
            model.IgnoreDamageReduction = value;
            return;
        }

        if (LevenshteinWithin(name, "шансоглушения", "stunaccuracy"))
        {
            model.StunChancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениеоглушению", "stunresistance"))
        {
            model.StunResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениеудержанию", "holdresistance"))
        {
            model.HoldResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениеагрессии", "aggressionresistance"))
        {
            model.AggroResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "сопротивлениебезмолвию", "silenceresistance"))
        {
            model.SilenceResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "ccaccuracy", "шансаномальныхсостояний"))
        {
            model.AbnormalStatusChancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "ccresistance", "сопраномальнымсостояниям"))
        {
            model.AbnormalStatusResistancePercent = value;
            return;
        }

        if (LevenshteinWithin(name, "ccdurationreduction", "уменьшдлитаномальныхсостояний", "умендлитаномальныхсостояний"))
        {
            model.AbnormalStatusDurationReductionPercent = value;
            return;
        }
    }

    private bool IsValidParameter(string name)
    {
        if (LevenshteinWithin(name, "уронвдальнбою", "rangeddamage", "уронвближнбою", "meleedamage", "магурон", "magicdamage"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "точностьвдальнбою", "rangedaccuracy", "магточность", "magicaccuracy", "точностьвближнбою", "meleeaccuracy"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "критатквдальнбою", "rangedcriticalhit", "магкритатк", "критатквближнбою", "magiccriticalhit", "meleecriticalhit"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "допуронкритатк", "extradamageoncriticalhit"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "шансдвойногоурона", "doublechance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "шанстройногоурона", "triplechance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениедвойномуурону", "doubleresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениетройномуурону", "tripleresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "блокировкаоружия", "weaponblock"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "пробиваниеблока", "blockpenetration"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "увеличениеуронаоторужия", "weapondamageboost"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "увеличениеуронаотумений", "skilldamageboost"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "защита", "defense"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениеумениям", "skillresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениеуроноторужия", "weapondefense"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениеуронотумений", "skilldefense"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "игнорированиесниженияурона", "ignoredamagereduction"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "шансоглушения", "stunaccuracy"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениеоглушению", "stunresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениеудержанию", "holdresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениеагрессии", "aggressionresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "сопротивлениебезмолвию", "silenceresistance"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "ccaccuracy", "шансаномальныхсостояний"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "ccresistance", "сопраномальнымсостояниям"))
        {
            return true;
        }

        if (LevenshteinWithin(name, "ccdurationreduction", "уменьшдлитаномальныхсостояний", "умендлитаномальныхсостояний"))
        {
            return true;
        }

        return false;
    }

    private bool LevenshteinWithin(string parameterName, params string[] parameters) =>
        parameters.Any(p => LevenshteinWithin(parameterName, p, 1));

    private bool LevenshteinWithin(string s, string t, int maxDistance)
    {
        // оптимизация: считаем только полосу шириной maxDistance
        int n = s.Length, m = t.Length;
        if (n == 0) return m <= maxDistance;
        if (m == 0) return n <= maxDistance;

        // гарантируем что t короче/равно (чтобы меньше памяти)
        if (m > n) { (s, t) = (t, s); (n, m) = (m, n); }

        var prev = new int[m + 1];
        var curr = new int[m + 1];

        for (var j = 0; j <= m; j++) prev[j] = j;

        for (var i = 1; i <= n; i++)
        {
            curr[0] = i;

            var from = Math.Max(1, i - maxDistance);
            var to = Math.Min(m, i + maxDistance);

            // outside band -> set big values
            for (var j = 1; j < from; j++) curr[j] = maxDistance + 1;
            for (var j = to + 1; j <= m; j++) curr[j] = maxDistance + 1;

            var bestInRow = maxDistance + 1;

            for (var j = from; j <= to; j++)
            {
                var cost = AreSimilar(s[i - 1], t[j - 1]) ? 0 : 1;

                var del = prev[j] + 1;
                var ins = curr[j - 1] + 1;
                var sub = prev[j - 1] + cost;

                var v = Math.Min(Math.Min(del, ins), sub);
                curr[j] = v;
                if (v < bestInRow) bestInRow = v;
            }

            if (bestInRow > maxDistance) return false;
            (prev, curr) = (curr, prev);
        }

        return prev[m] <= maxDistance;
    }

    private bool AreSimilar(char a, char b)
    {
        if (a == b) return true;

        a = char.ToLowerInvariant(a);
        b = char.ToLowerInvariant(b);

        if (a == b) return true;

        if (SimilarChars.TryGetValue(a, out var listA) && listA.Contains(b))
            return true;

        if (SimilarChars.TryGetValue(b, out var listB) && listB.Contains(a))
            return true;

        return false;
    }

    private readonly Dictionary<char, char[]> SimilarChars = new()
    {
        // Latin -> Cyrillic
        ['a'] = ['а'],
        ['b'] = ['в'],
        ['c'] = ['с'],
        ['e'] = ['е'],
        ['h'] = ['н'],
        ['k'] = ['к'],
        ['m'] = ['м'],
        ['o'] = ['о'],
        ['p'] = ['р'],
        ['t'] = ['т'],
        ['x'] = ['х'],
        ['y'] = ['у'],
        ['r'] = ['г'], // OCR часто путает

        // Cyrillic -> Latin (симметрия)
        ['а'] = ['a'],
        ['в'] = ['b'],
        ['с'] = ['c'],
        ['е'] = ['e'],
        ['н'] = ['h'],
        ['к'] = ['k'],
        ['м'] = ['m'],
        ['о'] = ['o'],
        ['р'] = ['p'],
        ['т'] = ['t'],
        ['х'] = ['x'],
        ['у'] = ['y'],
        ['г'] = ['r'],
    };
}