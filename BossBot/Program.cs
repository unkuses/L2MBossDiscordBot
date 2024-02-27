using Newtonsoft.Json;

namespace BossBot;

internal class Program
{
    private static void Main(string[] args)
    {
        var options = JsonConvert.DeserializeObject<Options>(File.ReadAllText("Options.ini"));
        if (options == null || String.IsNullOrWhiteSpace(options.BotToken) ||
            String.IsNullOrWhiteSpace(options.ChatName))
        {
            Console.WriteLine("Cannot find options or they are empty");
            return;
        }

        var discordRuntime = new DiscordRuntime(options);
        discordRuntime.LogIn();
        Console.ReadKey();
    }
}