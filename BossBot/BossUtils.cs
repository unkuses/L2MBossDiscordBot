using System.Text;
using BossBot.Interfaces;
using CommonLib.Helpers;
using CommonLib.Models;

namespace BossBot;

public class BossUtils(ILanguage localization)
{
    public Task<List<string>> PopulateBossInformationString(ulong chatId, IList<BossModel> models,
        DateTimeHelper dateTimeHelper)
    {
        List<StringBuilder> builders = [];
        var stringBuilder = new StringBuilder();
        builders.Add(stringBuilder);
        if (models.Count == 0)
        {
            stringBuilder.AppendLine(localization.BossListEmpty(chatId));
        }
        else
        {
            var maxLength = models.Max(x => x.NickName.Length);
            for(var i = 0;i < models.Count;)
            {
                var followingBosses = FollowingBosses(models, i);
                if (followingBosses < 5)
                {
                    PopulateString(builders, ref stringBuilder, CreateBossInfoString(dateTimeHelper, models[i], maxLength));
                    i++;
                }
                else
                {   
                    List<string> bossInfos = [];
                    var bossLocation = new Dictionary<string, int>();
                    var chainEnd = i + followingBosses;
                    for (; i < chainEnd && i < models.Count; i++)
                    {
                        if(!bossLocation.TryAdd(models[i].Location, 0))
                            bossLocation[models[i].Location]++;

                        bossInfos.Add(CreateBossInfoString(dateTimeHelper, models[i], maxLength));
                    }

                    var maxCount = bossLocation.Values.Max();
                    var maxLocations = bossLocation.First(l => l.Value == maxCount).Key;
                    PopulateString(builders, ref stringBuilder, localization.ChainBeginAnnouncement(chatId, maxLocations));
                    foreach (var bossInfo in bossInfos)
                    {
                        PopulateString(builders, ref stringBuilder, bossInfo);
                    }
                    PopulateString(builders, ref stringBuilder, localization.ChainEndAnnouncement(chatId));
                }
            }
        }

        return Task.FromResult(builders.Select(b => b.ToString()).ToList());
    }

    private static int FollowingBosses(IList<BossModel> models, int currentModel)
    {
        var result = 1;
        var prevRespawn = models[currentModel].NextRespawnTime;

        for (var i = currentModel + 1; i < models.Count; i++)
        {
            if (prevRespawn.AddMinutes(5) > models[i].NextRespawnTime)
            {
                result++;
                prevRespawn = models[i].NextRespawnTime;
            }
            else
            {
                break;
            }
        }
        return result;
    }

    private static string CreateBossInfoString(DateTimeHelper dateTimeHelper,  BossModel model, int maxLength)
    {
        var nextRespawnTime = model.KillTime.AddHours(model.RespawnTime);
        var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
        return $@"**{StringHelper.PopulateWithWhiteSpaces(model.Id, 2)}**|{nextRespawnTime:HH:mm}|**{StringHelper.PopulateWithWhiteSpaces(model.NickName.ToUpper(), maxLength)}** через {timeToRespawn:hh\:mm} | {model.Chance}{GetChanceStatus(model.Chance)}{AppendEggPlant(model.PurpleDrop)}";
    }

    private static void PopulateString(List<StringBuilder> builders, ref StringBuilder stringBuilder, string str)
    {
        if (stringBuilder.Length + str.Length > 2000)
        {
            stringBuilder = new StringBuilder();
            builders.Add(stringBuilder);
        }
        stringBuilder.AppendLine(str);
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
