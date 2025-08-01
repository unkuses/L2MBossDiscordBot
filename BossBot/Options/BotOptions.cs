namespace BossBot.Options;

public class BotOptions(string chatName, string botToken, string imageAnalysisURL, string imageStatisticAnalysisUrl, string cosmoDbUrl, string cosmoDbKey, string chatEvent, string openAIEndpoint, string openAIKey, string timeZone = "Russian Standard Time")
{
    public string ChatName { get; set; } = chatName;

    public string ChatEvent { get; set; } = string.IsNullOrEmpty(chatEvent) ? "события-будильники" : chatEvent;

    public string BotToken { get; set; } = botToken;

    public string TimeZone { get; set; } = timeZone;

    public string ImageAnalysisUrl { get; set; } = imageAnalysisURL;

    public string ImageStatisticAnalysisUrl { get; set; } = imageStatisticAnalysisUrl;

    public string CosmoDbUrl { get; set; } = cosmoDbUrl;

    public string CosmoDbKey { get; set; } = cosmoDbKey;

    public string OpenAIEnpoint { get; set; } = openAIEndpoint;

    public string openAIKey { get; set; } = openAIKey;
}