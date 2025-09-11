using BossBot.Interfaces;
using BossBot.Localization;
using CommonLib.Models;

namespace BossBot.Service;

public class Localization(ChatLanguageData chatLanguageData) : ILanguage
{
    private readonly ILanguage _ruLanguage = new RuLanguage();
    private readonly ILanguage _uaLanguage = new UALanguage();

    public string ClearAllBosses(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.ClearAllBosses(chatId),
            "ua" => _uaLanguage.ClearAllBosses(chatId),
            _ => _ruLanguage.ClearAllBosses(chatId)
        };
    }

    public string IncorrectFormat(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.IncorrectFormat(chatId),
            "ua" => _uaLanguage.IncorrectFormat(chatId),
            _ => _ruLanguage.IncorrectFormat(chatId)
        };
    }

    public string BossLogged(ulong chatId, BossModel bossModel, DateTime nextRespawnTime, TimeSpan timeToRespawn)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.BossLogged(chatId, bossModel, nextRespawnTime, timeToRespawn),
            "ua" => _uaLanguage.BossLogged(chatId, bossModel, nextRespawnTime, timeToRespawn),
            _ => _ruLanguage.BossLogged(chatId, bossModel, nextRespawnTime, timeToRespawn)
        };
    }

    public string BossNotFound(ulong chatId, string id)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.BossNotFound(chatId, id),
            "ua" => _uaLanguage.BossNotFound(chatId, id),
            _ => _ruLanguage.BossNotFound(chatId, id)
        };
    }

    public string ChatDeleted(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.ChatDeleted(chatId),
            "ua" => _uaLanguage.ChatDeleted(chatId),
            _ => _ruLanguage.ChatDeleted(chatId)
        };
    }

    public string BossNewTime(ulong chatId, string id, string name, DateTime newTime, TimeSpan timeToRespawn)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.BossNewTime(chatId, id, name, newTime, timeToRespawn),
            "ua" => _uaLanguage.BossNewTime(chatId, id, name, newTime, timeToRespawn),
            _ => _ruLanguage.BossNewTime(chatId, id, name, newTime, timeToRespawn)
        };
    }

    public string BossRespawnTimeUpdatedAnnouncement(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.BossRespawnTimeUpdatedAnnouncement(chatId),
            "ua" => _uaLanguage.BossRespawnTimeUpdatedAnnouncement(chatId),
            _ => _ruLanguage.BossRespawnTimeUpdatedAnnouncement(chatId)
        };
    }

    public string ChainBeginAnnouncement(ulong chatId, string location)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.ChainBeginAnnouncement(chatId, location),
            "ua" => _uaLanguage.ChainBeginAnnouncement(chatId, location),
            _ => _ruLanguage.ChainBeginAnnouncement(chatId, location)
        };
    }

    public string ChainEndAnnouncement(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.ChainEndAnnouncement(chatId),
            "ua" => _uaLanguage.ChainEndAnnouncement(chatId),
            _ => _ruLanguage.ChainEndAnnouncement(chatId)
        };
    }

    public string BossListEmpty(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.BossListEmpty(chatId),
            "ua" => _uaLanguage.BossListEmpty(chatId),
            _ => _ruLanguage.BossListEmpty(chatId)
        };
    }

    public string UpcomingBossesAnnouncement(ulong chatId)
    {
        var language = chatLanguageData.GetLanguage(chatId);
        return language.ToLower() switch
        {
            "ru" => _ruLanguage.UpcomingBossesAnnouncement(chatId),
            "ua" => _uaLanguage.UpcomingBossesAnnouncement(chatId),
            _ => _ruLanguage.UpcomingBossesAnnouncement(chatId)
        };
    }
}