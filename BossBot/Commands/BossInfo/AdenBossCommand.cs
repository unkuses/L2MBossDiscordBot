using BossBot.Interfaces;
using CommonLib.Helpers;

namespace BossBot.Commands.BossInfo;

public class AdenBossCommand(CosmoDb bossData, DateTimeHelper dateTimeHelper) : ICommand
{
    public string[] Keys { get; } = ["aden", "аден"];

    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var bossesInAden = await bossData.GetAllBossesByLocationAsync(chatId, "Aden");
        return await BossUtils.PopulateBossInformationString(bossesInAden, dateTimeHelper);
    }
}