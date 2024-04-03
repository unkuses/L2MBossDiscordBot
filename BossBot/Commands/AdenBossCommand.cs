using System.Text;
using BossBot.Interfaces;

namespace BossBot.Commands;

public class AdenBossCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
{
    public string[] Keys { get; } = ["aden", "аден"];

    public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
    {
        var bossesInAden = bossData.GetAllAdenByLocation(chatId, "Aden");
        return BossUtils.PopulateBossInformationString(bossesInAden, dateTimeHelper);
    }
}