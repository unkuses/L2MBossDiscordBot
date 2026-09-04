namespace BossBot.Services.Model;

public class EventActivity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Count { get; set; }
    public string EventName { get; set; } = string.Empty;

    public Player User { get; set; } = null!;
}
