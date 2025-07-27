using BossBot.Interfaces;

namespace BossBot.Commands;

public class RegisterChatCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = ["RegisterChatAsBossBot".ToLower()];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        bossData.RegisterChat(chatId);
        return Task.FromResult<List<string>>(["Chat registered successfully."]);
    }
}