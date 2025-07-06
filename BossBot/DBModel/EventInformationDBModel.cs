using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BossBot.DBModel;

public class EventInformationDBModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int EventNumber { get;set; }
    public DateTime Time { get; set; }

    public string EventName { get; set; }

    public RepeatDays Days { get; set; }

    public ulong ChatId { get; set; }

    [DefaultValue(false)] public bool IsOneTimeEvent { get; set; } = false;

    [DefaultValue(5)] public double TimeBeforeNotification { get; set; } = 5;
}

[Flags]
public enum RepeatDays
{
    None = 0,
    Sun = 1 << 0, // Sunday
    Mon = 1 << 1, // Monday
    Tue = 1 << 2, // Tuesday
    Wed = 1 << 3, // Wednesday
    Thu = 1 << 4, // Thursday
    Fri = 1 << 5, // Friday
    Sat = 1 << 6  // Saturday
}