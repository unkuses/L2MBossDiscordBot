using BossBot.Interfaces;
using CommonLib.Helpers;

namespace BossBot.Commands;

public class GetAllBossCommand(CosmoDb bossData, DateTimeHelper dateTimeHelper) : ICommand
{
    public string[] Keys { get; } =
    [
        "l", "л"
    ]; 
    
    public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (commands.Length == 1 || !int.TryParse(commands[1], out var count))
        {
            return GetBossInformation(chatId);
        }
        else
        {
            return GetFirstLoggedBossInfo(chatId, count);
        }
    }

    private async Task<IEnumerable<string>> GetBossInformation(ulong chatId)
    {
        var bosses = await bossData.GetAllLoggedBossInfoAsync(chatId);
        return await BossUtils.PopulateBossInformationString(bosses, dateTimeHelper);
    }

    private async Task<IEnumerable<string>> GetFirstLoggedBossInfo(ulong chatId, int count)
    {
        if (count <= 0)
        {
            return [];
        }

        var bosses = await bossData.GetFirstLoggedBossInfoAsync(chatId, count);
        return await BossUtils.PopulateBossInformationString(bosses, dateTimeHelper);
    }
}
