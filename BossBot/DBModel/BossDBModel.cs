using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BossBot.DBModel
{
    public class BossDbModel
    {
        public BossDbModel()
        {
        }

        [SetsRequiredMembers]
        public BossDbModel(string name, int chance, string location, string nickName, int respawnTime, int restartRespawnTime = 0)
        {
            Name = name;
            Chance = chance;
            Location = location;
            NickName = nickName;
            RespawnTime = respawnTime;
            RestartRespawnTime = restartRespawnTime;
        }

        [Key]
        public int ID { get; set; }

        public required string Name { get; set; }
        public required string NickName { get; set; }
        public required int Chance { get; set; }
        public required int RespawnTime { get; set; }
        public required string Location { get; set; }

        [DefaultValue(4)] public required int RestartRespawnTime { get; set; }
        public ICollection<BossInformationDbModel> BossInformationDbModels { get; set; }
    }
}
