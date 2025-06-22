using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BossBot.DBModel;

public class EventInformationDBModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime Time { get; set; }

    public string EventName { get; set; }

    public RepeatDays Days { get; set; }

    public ulong ChatId { get; set; }
}

[Flags]
public enum RepeatDays
{
    None = 0,
    Su = 1 << 0, // Sunday
    M = 1 << 1, // Monday
    Tu = 1 << 2, // Tuesday
    W = 1 << 3, // Wednesday
    Th = 1 << 4, // Thursday
    F = 1 << 5, // Friday
    Sa = 1 << 6  // Saturday
}