namespace BossBot
{
    public class Options(string chatName, string botToken, string imageAnalysisURL, string imageAnalysisKey,  string timeZone = "Russian Standard Time")
    {
        public string ChatName { get; set; } = chatName;

        public string BotToken { get; set; } = botToken;

        public string TimeZone { get; set; } = timeZone;

        public string ImageAnalysisUrl { get; set; } = imageAnalysisURL;

        public string ImageAnalysisKey { get; set; } = imageAnalysisKey;
    }
}