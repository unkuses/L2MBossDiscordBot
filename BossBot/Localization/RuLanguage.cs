using BossBot.Interfaces;
using CommonLib.Helpers;
using CommonLib.Models;

namespace BossBot.Localization;

public class RuLanguage : ILanguage
{
    public string ClearAllBosses(ulong chatId) => "Все тайминги были сброшены";
    public string IncorrectFormat(ulong chatId) => "Неправильный формат команды";

    public string BossNotFound(ulong chatId, string bossId) => $"Босс с номером {bossId} не был найден";

    public string BossLogged(ulong chatId, BossModel boss, DateTime nextRespawnTime, TimeSpan timeToRespawn) =>
        $"Босс убит **{boss.Id}** **{boss.NickName.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}";

    public string ChatDeleted(ulong chatId) => "Чат был удален, информация о боссах очищена";
    
    public string BossNewTime(ulong chatId, string id, BossModel bossModel, DateTime newTime, TimeSpan timeToRespawn)
    => $"Босс **{StringHelper.PopulateWithWhiteSpaces(id, 2)}** **{bossModel.NickName.ToUpper()}** не был залогирован. Новое время {newTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}";

    public string UpcomingBossesAnnouncement(ulong chatId) => "@here Ближайшие боссы";

    public string BossRespawnTimeUpdatedAnnouncement(ulong chatId) => "Время респавна боссов были обновлены";

    public string BossListEmpty(ulong chatId) => "Нет боссов в списке";

    public string ChainBeginAnnouncement(ulong chatId, string location) =>
        $"\r\n--------Начало ** {location} ** чейна--------";

    public string ChainEndAnnouncement(ulong chatId) => "--------Конец чейна-------- \r\n";
}