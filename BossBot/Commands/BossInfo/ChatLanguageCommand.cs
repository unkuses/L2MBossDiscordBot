using BossBot.Interfaces;

namespace BossBot.Commands.BossInfo;

public class ChatLanguageCommand(ChatLanguageData chatLanguageData) : ICommand
{
    public string[] Keys { get; } = ["language", "мова"];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (commands[1].ToLower() == "ua")
        {
            chatLanguageData.UpdateLanguageSettings(chatId, "ua");
            return Task.FromResult(new List<string> { "Гарного полювання шановане товориство" });
        }
        else if (commands[1].ToLower() == "ru")
        {
            chatLanguageData.UpdateLanguageSettings(chatId, "ru");
            return Task.FromResult(new List<string> { "Язык установлен - русский" });
        }
        else
        {
            return Task.FromResult(new List<string> { "Invalid language. Use 'ua' for Ukrainian" });
        }
    }
}