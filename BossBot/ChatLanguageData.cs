using BossBot.DataSource;

namespace BossBot;

public class ChatLanguageData
{
    private readonly LanguageDataSource _languageDataSource = new();
    public ChatLanguageData()
    {
        _languageDataSource.Database.EnsureCreated();
    }

    public string GetLanguage(ulong chatId)
    {
        var lang = _languageDataSource.ChatLanguageInfo.FirstOrDefault(c => c.ChatId == chatId);
        return lang?.LanguageCode ?? "ru";
    }

    public bool UpdateLanguageSettings(ulong chatId, string languageCode)
    {
        var lang = _languageDataSource.ChatLanguageInfo.FirstOrDefault(c => c.ChatId == chatId);
        if (lang != null)
        {
            lang.LanguageCode = languageCode;
            _languageDataSource.ChatLanguageInfo.Update(lang);
        }
        else
        {
            _languageDataSource.ChatLanguageInfo.Add(new DBModel.ChatLanguageDBModel() { ChatId = chatId, LanguageCode = languageCode });
        }
        return _languageDataSource.SaveChanges() > 0;
    }
}