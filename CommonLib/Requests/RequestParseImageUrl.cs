namespace CommonLib.Requests;

public class RequestParseImageUrl
{
    public string Url { get; set; }
    public string TimeZone { get; set; }
    public ulong ChatId { get; set; }

    public string Language { get; set; } = "";
}