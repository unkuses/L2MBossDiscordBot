using BossBot.Interfaces;

namespace BossBot.Commands;

public class UnregisterChatCommand(BossData bossData, CosmoDb cosmoDb) : ICommand
{
    public string[] Keys { get; } =  ["DisableBossChat".ToLower()];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        bossData.UnregisterChat(chatId);
        await cosmoDb.ClearAllBossInformationAsync(chatId);
        return ["Чат был удален, информация о боссах очищена"];
    }
}