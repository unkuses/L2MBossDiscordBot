using System.ComponentModel.DataAnnotations;

namespace BossBot.DBModel;

public class UserStatisticDBModel
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public ulong ChatId { get; set; }

    public string UserName { get; set; }

    public int Count { get; set; }

}