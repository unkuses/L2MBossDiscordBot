using System.ComponentModel.DataAnnotations;

namespace BossBot.DBModel;

public class BossBotChatsDBModel
{
    [Key]
    public ulong ChatId { get; set; }
}