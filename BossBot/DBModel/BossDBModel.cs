using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossBot.DBModel
{
    public class BossDbModel
    {
        public BossDbModel()
        {
        }

        [SetsRequiredMembers]
        public BossDbModel(string name, int chance, string location, string nickName, int respawnTime)
        {
            Name = name;
            Chance = chance;
            Location = location;
            NickName = nickName;
            RespawnTime = respawnTime;
        }

        [Key]
        public int ID { get; set; }

        public required string Name { get; set; }
        public required string NickName { get; set; }
        public required int Chance { get; set; }
        public required int RespawnTime { get; set; }
        public required string Location { get; set; }
        public ICollection<BossInformationDbModel> BossInformationDbModels { get; set; }
    }
}
