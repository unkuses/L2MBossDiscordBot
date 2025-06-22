using Newtonsoft.Json;

namespace BossBot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var botTok = Environment.GetEnvironmentVariable("BotKey");
        var chatName = Environment.GetEnvironmentVariable("ChatBotName");
        Options options;
        if (!string.IsNullOrWhiteSpace(botTok) && string.IsNullOrWhiteSpace(chatName))
        {
            options = new Options(chatName, botTok, "", "", "", "");
        }
        else
        {
            options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("Options.ini"));
        }

        if (options == null || string.IsNullOrWhiteSpace(options.BotToken) ||
            string.IsNullOrWhiteSpace(options.ChatName))
        {
            Console.WriteLine("Cannot find options or they are empty");
            return;
        }

        var discordRuntime = new DiscordRuntime(options);
        await discordRuntime.LogIn();
        await discordRuntime.MaintenanceTask();
        while (true)
        {
            Thread.Sleep(1000*60);
        }
    }
}