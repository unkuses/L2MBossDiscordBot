namespace BossBot.DBModel;
using System.ComponentModel.DataAnnotations;

public class UserInformationDBModel
{
    [Key]
    public ulong UserId { get; set; }
    
    public string UserTimeZone { get; set; }
}