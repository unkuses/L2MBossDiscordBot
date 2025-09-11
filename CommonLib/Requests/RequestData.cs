namespace CommonLib.Requests;

public class RequestData
{
    public string TimeZone { get; set; }
    public byte[] Image { get; set; }
    public ulong ChatId { get; set; }

    public string Language { get; set; } = "";
}