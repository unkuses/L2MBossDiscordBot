namespace CommonLib.Requests;

public class RequestGetStatistic
{
    public string EventName { get; set; } = string.Empty;

    public ulong ChatId { get; set; }
}
