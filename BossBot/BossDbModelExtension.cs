using BossBot.DBModel;

namespace BossBot;

public static class BossDbModelExtension
{
    public static bool AreEqual(this BossDbModel original, BossDbModel model)
    {
        return original.Name == model.Name && original.NickName == model.NickName && original.Chance == model.Chance &&
               original.RespawnTime == model.RespawnTime && original.Location == model.Location &&
               original.RestartRespawnTime == model.RestartRespawnTime;
    }
}