using BossBot.Interfaces;
using CommonLib.Helpers;
using CommonLib.Models;

namespace BossBot.Localization;

public class UALanguage : ILanguage
{
    public string ClearAllBosses(ulong chatId) => "Всі таймінги були скинуті";

    public string IncorrectFormat(ulong chatId) => "Невірний формат команди";

    public string BossNotFound(ulong chatId, string bossId) => $"Бос з номером {bossId} не був знайдений";
    
    public string BossLogged(ulong chatId, BossModel boss, DateTime nextRespawnTime, TimeSpan timeToRespawn) =>
        $"Бос вбитий **{boss.Id}** **{boss.Name.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}";
    
    public string ChatDeleted(ulong chatId) => "Чат був видалений, інформація про босів очищена";
    
    public string BossNewTime(ulong chatId, string id, BossModel bossModel, DateTime newTime, TimeSpan timeToRespawn)
        => $"Бос **{id}** **{bossModel.Name.ToUpper()}** не був залогований. Новий час {newTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}";

    public string AppendingBoss(ulong chatId, BossModel bossModel, DateTime nextRespawnTime, TimeSpan timeToRespawn) =>
        $"**{StringHelper.PopulateWithWhiteSpaces(bossModel.Id, 2)}**|{nextRespawnTime:HH:mm}|**{bossModel.Name.ToUpper()}**| через {timeToRespawn.ToString(@"hh\:mm")} | {bossModel.Chance} {BossUtils.GetChanceStatus(bossModel.Chance)}{BossUtils.AppendEggPlant(bossModel.PurpleDrop)}";

    public string UpcomingBossesAnnouncement(ulong chatId) => "@here Найближчі боси";

    public string BossRespawnTimeUpdatedAnnouncement(ulong chatId) => "Час респавна босів був оновлений";

    public string BossListEmpty(ulong chatId) => "Нема босів у списку";

    public string ChainBeginAnnouncement(ulong chatId, string location) =>
        $"\r\n--------Початок ** {location} ** чейну--------";

    public string ChainEndAnnouncement(ulong chatId) => "--------Кінець чейну-------- \r\n";
}