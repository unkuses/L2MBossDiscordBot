namespace CommonLib.Requests;

public class RequestAddUserToEvent
{
    public string Url { get; set; }
    public ulong ChatId { get; set; }

    public string EventName { get; set; }
}