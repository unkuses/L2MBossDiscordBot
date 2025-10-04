using CommonLib.Models;

namespace BossBot.Interfaces;

public interface ILanguage
{
    string ClearAllBosses(ulong chatId);

    string IncorrectFormat(ulong chatId);

    string BossNotFound(ulong chatId, string bossId);

    string BossLogged(ulong chatId, BossModel bossModel, DateTime nextRespawnTime, TimeSpan timeToRespawn);

    string ChatDeleted(ulong chatId);

    string BossNewTime(ulong chatId, string id, BossModel bossModel, DateTime newTime, TimeSpan timeToRespawn);

    string UpcomingBossesAnnouncement(ulong chatId);

    string BossRespawnTimeUpdatedAnnouncement(ulong chatId);

    string BossListEmpty(ulong chatId);

    string ChainBeginAnnouncement(ulong chatId, string location);

    string ChainEndAnnouncement(ulong chatId);
}