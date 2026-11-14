namespace CommonLib.Requests;

public class RequestGetUserStatistic
{
    public ulong ChatId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
