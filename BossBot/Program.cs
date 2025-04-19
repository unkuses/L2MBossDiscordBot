using Newtonsoft.Json;

namespace BossBot;

internal class Program
{
    private static void Main(string[] args)
    {
        var botTok = Environment.GetEnvironmentVariable("BotKey");
        var chatName = Environment.GetEnvironmentVariable("ChatBotName");
        Options options;
        if (!string.IsNullOrWhiteSpace(botTok) && string.IsNullOrWhiteSpace(chatName))
        {
            options = new Options(chatName, botTok, "", "", "");
        }
        else
        {
            options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("Options.ini"));
        }

        if (options == null || String.IsNullOrWhiteSpace(options.BotToken) ||
            String.IsNullOrWhiteSpace(options.ChatName))
        {
            Console.WriteLine("Cannot find options or they are empty");
            return;
        }

        var discordRuntime = new DiscordRuntime(options);
        discordRuntime.LogIn();
        RunServiceLoop(CancellationToken.None);
    }

    private static void RunServiceLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // Do background work here
            Thread.Sleep(1000*360); // Simulate work
        }
    }
}