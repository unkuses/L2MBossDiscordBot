namespace BossBot
{
    public class Options(string chatName, string botToken, string imageAnalysisURL, string cosmoDbUrl, string cosmoDbKey, string timeZone = "Russian Standard Time")
    {
        public string ChatName { get; set; } = chatName;

        public string BotToken { get; set; } = botToken;

        public string TimeZone { get; set; } = timeZone;

        public string ImageAnalysisUrl { get; set; } = imageAnalysisURL;

        public string CosmoDbUrl { get; set; } = cosmoDbUrl;

        public string CosmoDbKey { get; set; } = cosmoDbKey;
    }
}