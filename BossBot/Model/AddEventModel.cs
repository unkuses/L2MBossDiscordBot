using BossBot.DBModel;

namespace BossBot.Model;

public class AddEventModel
{
    public RepeatDays RepeatAt { get; set; }

    public DateTime Time { get; set; }

    public string Description { get; set; } = string.Empty;

    public int TimeBeforeNotification { get; set; }
}
