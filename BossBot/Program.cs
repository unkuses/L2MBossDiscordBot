using Newtonsoft.Json;

namespace BossBot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("Options.ini"));

        if (options == null || string.IsNullOrWhiteSpace(options.BotToken) ||
            string.IsNullOrWhiteSpace(options.ChatName))
        {
            Console.WriteLine("Cannot find options or they are empty");
            return;
        }

        var discordRuntime = new DiscordRuntime(options);
        await discordRuntime.LogIn();
        _ = discordRuntime.StartDailyJob();
        await discordRuntime.MaintenanceTask();
        while (true)
        {
            Thread.Sleep(1000*60);
        }
    }
}