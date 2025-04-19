using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CommonLib.DBModels;

public class BossDbModel
{
    public BossDbModel()
    {
    }

    [SetsRequiredMembers]
    public BossDbModel(int id, string name, int chance, string location, string nickName, int respawnTime, bool purpleDrop = false, int restartRespawnTime = 0)
    {
        Id = id.ToString();
        BossId = $"boss{id}";
        Name = name;
        Chance = chance;
        Location = location;
        NickName = nickName;
        RespawnTime = respawnTime;
        RestartRespawnTime = restartRespawnTime;
        PurpleDrop = purpleDrop;
    }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty(nameof(BossId))]
    public string BossId { get; set; }
    public required string Name { get; set; }
    public required string NickName { get; set; }
    public required int Chance { get; set; }
    public required int RespawnTime { get; set; }
    public required string Location { get; set; }
    public required bool PurpleDrop { get; set; }
        
    [DefaultValue(0)] public required int RestartRespawnTime { get; set; }
    public ICollection<BossInformationDbModel> BossInformationDbModels { get; set; }
    public ICollection<BossNamesDBModel> BossNames { get; set; }
}