using CommonLib.Helpers;
using CommonLib.Models;
using System.Text;
using BossBot.Interfaces;

namespace BossBot.Service;

public class RuntimeService(CosmoDb cosmoDb, BossData bossData, DiscordClientService discordClientService, DateTimeHelper dateTimeHelper, ILanguage localization)
{
    public async Task MaintenanceTask()
    {
        while (true)
        {
            await PostponeBossesAsync();
            await AppendingBossesAsync();
            await MentionAllNotAnnouncedBossesTask();
            UpcomingEvents();

            Thread.Sleep(60 * 1000);
        }
    }

    private async Task PostponeBossesAsync()
    {
        var postponeBosses = await cosmoDb.GetAndUpdateAllPostponeBossesAsync();

        if (postponeBosses.Count > 0)
        {
            Dictionary<ulong, IList<BossModel>> dic = new();
            foreach (var postponeBoss in postponeBosses)
            {
                if (!dic.ContainsKey(postponeBoss.ChatId.Value))
                {
                    dic[postponeBoss.ChatId.Value] = new List<BossModel>();
                }

                dic[postponeBoss.ChatId.Value].Add(postponeBoss);
            }

            foreach (var i in dic.Keys)
            {
                var builder = new StringBuilder();
                foreach (var item in dic[i])
                {
                    var nextRespawnTime = item.KillTime.AddHours(item.RespawnTime);
                    var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
                    builder.AppendLine(localization.BossNewTime(i, item.Id, item.NickName, nextRespawnTime,
                        timeToRespawn));
                }

                var channel = discordClientService.GetChannel(i);
                channel?.SendMessageAsync(builder.ToString());
            }
        }
    }

    public async Task AppendingBossesAsync()
    {
        var appendBosses = await cosmoDb.GetAllAppendingBossesAsync();
        if (appendBosses.Count > 0)
        {
            Dictionary<ulong, IList<BossModel>> dictionary = new();
            foreach (var appendBoss in appendBosses)
            {
                if (!dictionary.ContainsKey(appendBoss.ChatId.Value))
                {
                    dictionary[appendBoss.ChatId.Value] = new List<BossModel>();
                }

                dictionary[appendBoss.ChatId.Value].Add(appendBoss);
            }

            foreach (var i in dictionary.Keys)
            {
                var builder = new StringBuilder();
                builder.AppendLine(localization.UpcomingBossesAnnouncement(i));
                foreach (var item in dictionary[i])
                {
                    var nextRespawnTime = item.KillTime.AddHours(item.RespawnTime);
                    var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
                    builder.AppendLine(
                        $"**{StringHelper.PopulateWithWhiteSpaces(item.Id, 2)}**|{nextRespawnTime:HH:mm}|**{item.NickName.ToUpper()}**| через {timeToRespawn.ToString(@"hh\:mm")} | {item.Chance} {BossUtils.GetChanceStatus(item.Chance)}{BossUtils.AppendEggPlant(item.PurpleDrop)}");
                }

                var channel = discordClientService.GetChannel(i);
                channel?.SendMessageAsync(builder.ToString());
            }
        }
    }

    private void UpcomingEvents()
    {
        var upcomingEvents = bossData.GetAllEvents();
        if (!upcomingEvents.Any()) return;
        
        foreach (var upcomingEvent in upcomingEvents)
        {
            var channel = discordClientService.GetChannel(upcomingEvent.ChatId);

            channel?.SendMessageAsync(
                $"@here **{upcomingEvent.EventName}** в {upcomingEvent.Time:HH:mm} через {TimeDifference(upcomingEvent.Time)} минут.");
        }
    }

    private async Task MentionAllNotAnnouncedBossesTask()
    {
        var result = await cosmoDb.GetAllNotAnnouncedBossesAsync();
        if (result.Count > 0)
        {
            Dictionary<ulong, IList<BossModel>> dictionary = new();
            foreach (var appendBoss in result)
            {
                if (!dictionary.ContainsKey(appendBoss.ChatId.Value))
                {
                    dictionary[appendBoss.ChatId.Value] = new List<BossModel>();
                }

                dictionary[appendBoss.ChatId.Value].Add(appendBoss);
            }

            foreach (var i in dictionary.Keys)
            {
                var builder = new StringBuilder();
                builder.AppendLine(localization.BossRespawnTimeUpdatedAnnouncement(i));
                foreach (var item in dictionary[i])
                {
                    var nextRespawnTime = item.KillTime.AddHours(item.RespawnTime);
                    var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
                    builder.AppendLine(localization.BossLogged(i, item, nextRespawnTime, timeToRespawn));
                }

                var channel = discordClientService.GetChannel(i);
                channel?.SendMessageAsync(builder.ToString());
            }
        }
    }

    private int TimeDifference(DateTime time)
    {
        var now = dateTimeHelper.CurrentTime;
        var nowTime = new TimeSpan(now.Hour, now.Minute, 0);
        var eventTime = new TimeSpan(time.Hour, time.Minute, 0);

        // Calculate the difference in minutes
        return Convert.ToInt32((eventTime - nowTime).TotalMinutes);
    }
}