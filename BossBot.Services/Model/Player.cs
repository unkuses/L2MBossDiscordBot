namespace BossBot.Services.Model;

public class Player
{
    public Guid Id { get; set; }
    public ulong ChatId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();
}