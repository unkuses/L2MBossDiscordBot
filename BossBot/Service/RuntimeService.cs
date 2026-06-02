using BossBot.DBModel;
using BossBot.Interfaces;
using CommonLib.Helpers;
using CommonLib.Models;
using Discord;
using System.Text;

namespace BossBot.Service;

public class RuntimeService(CosmoDb cosmoDb, 
    BossData bossData, 
    DiscordClientService discordClientService, 
    DateTimeHelper dateTimeHelper, 
    ILanguage localization)
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

    public async Task StartDailyJob()
    {
        await Task.Delay(1000 * 60);
        while (true)
        {
            var now = dateTimeHelper.CurrentTime;
            var nextRun = now.Date.AddHours(9);
            if (now > nextRun)
                nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;
            await Task.Delay(delay);
            try
            {
                await GetAllDailyEvents();
            }
            catch (Exception)
            {

            }
        }
    }

    private async Task GetAllDailyEvents()
    {
        var events = bossData.GetAllTodayEvents();
        Dictionary<ulong, IList<EventInformationDBModel>> dictionary = new();
        foreach (var e in events)
        {
            if (!dictionary.ContainsKey(e.ChatId))
            {
                dictionary[e.ChatId] = new List<EventInformationDBModel>();
            }

            dictionary[e.ChatId].Add(e);
        }

        foreach (var chatId in dictionary.Keys)
        {
            var builder = new StringBuilder();
            builder.AppendLine("@here Ближайшие события:");
            foreach (var item in dictionary[chatId])
            {
                var timeToEvent = item.Time - dateTimeHelper.CurrentTime;
                builder.AppendLine($"**{item.EventName}** в {item.Time:HH:mm} через {timeToEvent.ToString(@"hh\:mm")}");
            }
            var channel = discordClientService.GetChannel(chatId);
            channel?.SendMessageAsync(builder.ToString());
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
                    builder.AppendLine(localization.BossNewTime(i, item.Id, item, nextRespawnTime,
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
                    builder.AppendLine(localization.AppendingBoss(i, item, nextRespawnTime, timeToRespawn));
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