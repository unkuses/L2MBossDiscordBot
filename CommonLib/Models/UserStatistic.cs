namespace CommonLib.Models;

public class UserStatistic
{
    public string PlayerName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<EventActivityStatistic> Events { get; set; } = [];
}

public class EventActivityStatistic
{
    public string EventName { get; set; } = string.Empty;
    public int Count { get; set; }
}
