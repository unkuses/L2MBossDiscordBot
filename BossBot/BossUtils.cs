using System.Text;
using CommonLib.Helpers;
using CommonLib.Models;

namespace BossBot;

public static class BossUtils
{
    public static Task<IEnumerable<string>> PopulateBossInformationString(IList<BossModel> models,
        DateTimeHelper dateTimeHelper)
    {
        List<StringBuilder> builders = new();
        var stringBuilder = new StringBuilder();
        builders.Add(stringBuilder);
        if (models.Count == 0)
        {
            stringBuilder.AppendLine("Нет боссов в списке");
        }
        else
        {
            int maxLength = models.Max(x => x.NickName.Length);
            foreach (var model in models)
            {
                var nextRespawnTime = model.KillTime.AddHours(model.RespawnTime);
                var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
                var str =
                    $@"**{StringHelper.PopulateWithWhiteSpaces(model.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{StringHelper.PopulateWithWhiteSpaces(model.NickName.ToUpper(), maxLength)}** через {timeToRespawn.ToString(@"hh\:mm")} | {model.Chance}{GetChanceStatus(model.Chance)}{AppendEggPlant(model.PurpleDrop)}";
                if (stringBuilder.Length + str.Length > 2000)
                {
                    stringBuilder = new StringBuilder();
                    builders.Add(stringBuilder);
                }

                stringBuilder.AppendLine(str);
            }
        }

        return Task.FromResult(builders.Select(b => b.ToString()));
    }

    public static string GetChanceStatus(int chance) =>
        chance switch
        {
            <= 33 => ":orange_circle:",
            50 => ":yellow_circle:",
            _ => ":green_circle:"
        };

    public static string AppendEggPlant(bool append) => append ? ":eggplant:" : "";
}