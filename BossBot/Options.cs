namespace BossBot
{
    public class Options(string chatName, string botToken, string timeZone = "Russian Standard Time")
    {
        public string ChatName { get; set; } = chatName;

        public string BotToken { get; set; } = botToken;

        public string TimeZone { get; set; } = timeZone;
    }
}