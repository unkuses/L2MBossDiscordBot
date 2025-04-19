using CommonLib.DBModels;

namespace CommonLib.Models
{
    public class BossModel(BossDbModel model)
    {
        public BossModel(BossInformationDbModel bossInformation) : this(bossInformation.Boss)
        {
            KillTime = bossInformation.KillTime;
            ChatId = bossInformation.ChatId;
        }

        public ulong? ChatId {get;}
        public string Id { get; } = model.Id;
        public string Name { get; } = model.Name;
        public string NickName { get; } = model.NickName;
        public int Chance {  get; } = model.Chance;
        public int RespawnTime { get; } = model.RespawnTime;

        public bool PurpleDrop { get; } = model.PurpleDrop;
        public DateTime KillTime { get; }
    }
}
