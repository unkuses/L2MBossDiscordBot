namespace CommonLib.Requests;

public class RequestParseImageUrl
{
    public string Url { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public ulong ChatId { get; set; }

    public string Language { get; set; } = "";
}
