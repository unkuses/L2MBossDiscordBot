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
            _ = ReadSheet(e.Item2, e.Item1.Attachments.Select(a => a.Url).ToList(), e.Item1.Author.GlobalName);
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
        name = name.Trim().ToLowerInvariant().Replace(" ", "");
        switch (name)
        {
            case "уронвближн.бою":
            case "meleedamage":
            case "маг.урон":
            case "magicdamage":
                model.Damage = value; break;

            case "маг.точность":
            case "magicaccuracy":
            case "точностьвближн.бою":
            case "meleeaccuracy":
                model.Accuracy = value; break;

            case "маг.крит.атк.":
            case "крит.атк.вближн.бою":
            case "magiccriticalhit":
            case "meleecriticalhit":
                model.CritAtkPercent = value; break;

            // ===== EXTRA / CRIT =====

            case "доп.уронкрит.атк.":
            case "extradamage(oncriticalhit)":
                model.BonusCritDamage = value; break;

            // ===== DOUBLE / TRIPLE =====
            case "шансдвойногоурона":
            case "doublechance":
                model.DoubleDamageChancePercent = value; break;

            case "шанстройногоурона":
            case "triplechance":
                model.TripleDamageChancePercent = value; break;

            case "сопротивлениедвойномуурону":
            case "doubleresistance":
                model.DoubleDamageResistancePercent = value; break;

            case "сопротивлениетройномуурону":
            case "tripleresistance":
                model.TripleDamageResistancePercent = value; break;

            // ===== BLOCK =====
            case "блокировкаоружия":
            case "weaponblock":
                model.WeaponBlockPercent = value; break;

            case "пробиваниеблока":
            case "blockpenetration":
                model.BlockPenetrationPercent = value; break;

            // ===== DAMAGE BOOST =====
            case "увеличениеуронаоторужия":
            case "weapondamageboost":
                model.WeaponDamageIncreasePercent = value; break;

            case "увеличениеуронаотумений":
            case "skilldamageboost":
                model.SkillDamageIncreasePercent = value; break;

            // ===== DEFENSE =====
            case "защита":
            case "defense":
                model.Defense = value; break;

            case "сопротивлениеумениям":
            case "skillresistance":
                model.SkillResistance = value; break;

            case "сопротивлениеуроноторужия":
            case "weapondefense":
                model.WeaponDamageResistancePercent = value; break;

            case "сопротивлениеуронотумений":
            case "skilldefense":
                model.SkillDamageResistancePercent = value; break;

            // ===== IGNORE =====
            case "игнорированиесниженияурона":
            case "ignoredamagereduction":
                model.IgnoreDamageReduction = value; break;

            // ===== CC / STUN =====
            case "шансоглушения":
            case "stunaccuracy":
                model.StunChancePercent = value; break;

            case "сопротивлениеоглушению":
            case "stunresistance":
                model.StunResistancePercent = value; break;

            case "сопротивлениеудержанию":
            case "holdresistance":
                model.HoldResistancePercent = value; break;

            case "сопротивлениеагрессии":
            case "aggressionresistance":
                model.AggroResistancePercent = value; break;

            case "сопротивлениебезмолвию":
            case "silenceresistance":
                model.SilenceResistancePercent = value; break;

            case "ccaccuracy":
            case "шансаномальныхсостояний":
                model.AbnormalStatusChancePercent = value; break;

            case "ccresistance":
            case "сопр.аномальнымсостояниям":
                model.AbnormalStatusResistancePercent = value; break;

            case "ccdurationreduction":
            case "уменьш.длит.аномальныхсостояний":
                model.AbnormalStatusDurationReductionPercent = value; break;
            default:
                // неизвестный стат — игнор или лог
                break;
        }
    }
}