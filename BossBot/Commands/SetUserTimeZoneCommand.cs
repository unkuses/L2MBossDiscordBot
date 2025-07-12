using System.Text;
using BossBot.Interfaces;

namespace BossBot.Commands;

public class SetUserTimeZoneCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = ["tz"];
    public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var stringBuilders = new List<StringBuilder>();
        var statusBuilder = new StringBuilder();
        stringBuilders.Add(statusBuilder);
        var tzList = commands.Where(s => s != "tz").ToList();
        var timeZoneId = string.Join(" ", tzList);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        statusBuilder.AppendLine(timeZone != null
            ? bossData.SetUserTimeZone(userId, timeZoneId)
            : "Incorrect format");

        return Task.FromResult(stringBuilders.Select(s => s.ToString()));
    }
}