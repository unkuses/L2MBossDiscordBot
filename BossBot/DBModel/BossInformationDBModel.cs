using System.ComponentModel.DataAnnotations;

namespace BossBot.DBModel
{
    public class BossInformationDbModel
    {
        [Key]
        public int Id { get; set; }
        public int BossId {  get; set; }
        public DateTime KillTime { get; set; } 
        public ulong ChatId { get; set; }

        public required BossDbModel Boss { get; set; }
    }
}
