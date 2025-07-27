using System.Text;
using BossBot.Interfaces;

namespace BossBot.Commands.BossInfo;

public class SetUserTimeZoneCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = ["tz"];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var tzList = commands.Where(s => s != "tz").ToList();
        var timeZoneId = string.Join(" ", tzList);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var response = timeZone != null
            ? bossData.SetUserTimeZone(userId, timeZoneId)
            : "Incorrect format";

        return Task.FromResult(new List<string> { response});
    }
}