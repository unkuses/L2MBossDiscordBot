using System.ComponentModel.DataAnnotations;

namespace BossBot.DBModel;

public class ChatLanguageDBModel
{
    [Key]
    public int Id { get; set; }
    public ulong ChatId { get; set; }
    public string LanguageCode { get; set; }
}