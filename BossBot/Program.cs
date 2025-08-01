using BossBot.Commands;
using BossBot.Commands.ActivityLogger;
using BossBot.Commands.BossInfo;
using BossBot.Commands.Event;
using BossBot.Options;
using BossBot.Service;
using CommonLib.Helpers;
using CommonLib.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace BossBot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var options = JsonConvert.DeserializeObject<BotOptions>(File.ReadAllText("Options.ini"));

        if (options == null || string.IsNullOrWhiteSpace(options.BotToken) ||
            string.IsNullOrWhiteSpace(options.ChatName))
        {
            Console.WriteLine("Cannot find options or they are empty");
            return;
        }
        
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(new DateTimeHelperOptions() { TimeZone = options.TimeZone });
        builder.Services.AddSingleton<DiscordRuntime>();
        builder.Services.AddSingleton<RuntimeService>();
        builder.Services.AddSingleton<DateTimeHelper>();
        builder.Services.AddSingleton(new CosmoDbOptions() { Endpoint = options.CosmoDbUrl, AccountKey = options.CosmoDbKey});
        builder.Services.AddSingleton<CosmoDb>();
        builder.Services.AddSingleton<OpenAIService>();
        builder.Services.AddSingleton<DiscordClientService>();
        builder.Services.AddSingleton<BossData>();
        builder.Services.AddSingleton<Logger>();
        builder.Services.AddSingleton<ActivityService>();
        builder.Services.AddSingleton<UserStatisticData>();

        #region Commands
        builder.Services.AddSingleton<RegisterChatCommand>();
        builder.Services.AddSingleton<ClearBossCommand>();
        builder.Services.AddSingleton<GetAllBossCommand>();
        builder.Services.AddSingleton<GetAllBossInformationCommand>();
        builder.Services.AddSingleton<GetAllNotLoggedBossesCommand>();
        builder.Services.AddSingleton<GetBossListWithKillTimeCommand>();
        builder.Services.AddSingleton<LogKillBossCommand>();
        builder.Services.AddSingleton<RestartTimeCommand>();
        builder.Services.AddSingleton<SetUserTimeZoneCommand>();
        builder.Services.AddSingleton<AddEventCommand>();
        builder.Services.AddSingleton<RemoveEventCommand>();
        builder.Services.AddSingleton<GetAllEventsCommand>();
        builder.Services.AddSingleton<EventChatService>();
        builder.Services.AddSingleton<BossChatService>();
        builder.Services.AddSingleton<AddUserCommand>();
        builder.Services.AddSingleton<ClearAllStatistic>();
        builder.Services.AddSingleton<GetUserStatistics>();
        builder.Services.AddSingleton<RemoveUserCommand>();
        builder.Services.AddSingleton<RegisterUser>();
        builder.Services.AddSingleton<MergeUsersCommand>();
        builder.Services.AddSingleton<UnregisterChatCommand>();
        #endregion

        var serviceProvider = builder.Services.BuildServiceProvider();
        var discordRuntime = serviceProvider.GetRequiredService<DiscordRuntime>();

        await discordRuntime.MaintenanceTask();
        while (true)
        {
            Thread.Sleep(1000*60);
        }
    }
}