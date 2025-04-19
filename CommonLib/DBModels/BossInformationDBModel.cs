using System.ComponentModel.DataAnnotations;

namespace CommonLib.DBModels;

public class BossInformationDbModel
{
    [Key]
    public string id { get; set; } = Guid.NewGuid().ToString();

    public required string BossInformationId { get; set; }
    public DateTime KillTime { get; set; } 
    public DateTime NextRespawnTime { get; set; }
    public ulong ChatId { get; set; }
    public required BossDbModel Boss { get; set; }

    public bool WasMentioned { get; set; } = true;
}