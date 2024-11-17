using BossBot.Interfaces;

namespace BossBot.Commands;

public class OrenBossCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
{
    public string[] Keys { get; } = ["oren", "орен"];

    public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var bosses = bossData.GetAllAdenByLocation(chatId, "Oren");
        return BossUtils.PopulateBossInformationString(bosses, dateTimeHelper);
    }
}