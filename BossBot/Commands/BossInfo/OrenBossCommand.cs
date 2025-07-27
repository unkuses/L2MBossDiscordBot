using BossBot.Interfaces;
using CommonLib.Helpers;

namespace BossBot.Commands.BossInfo;

public class OrenBossCommand(CosmoDb bossData, DateTimeHelper dateTimeHelper) : ICommand
{
    public string[] Keys { get; } = ["oren", "орен"];

    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var bosses = await bossData.GetAllBossesByLocationAsync(chatId, "Oren");
        return await BossUtils.PopulateBossInformationString(bosses, dateTimeHelper);
    }
}