using System.Text;
using BossBot.Interfaces;

namespace BossBot.Commands;

public class SetUserTimeZoneCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = new[] { "tz" };
    public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var stringBuilders = new List<StringBuilder>();
        var statusBuilder = new StringBuilder();
        stringBuilders.Add(statusBuilder);
        var timeZoneName = commands.Where(s => s != "tz").ToList();
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(string.Join(" ", timeZoneName));
        statusBuilder.AppendLine(timeZone != null
            ? bossData.SetUserTimeZone(userId, timeZone.StandardName)
            : "Incorrect format");

        return Task.FromResult(stringBuilders.Select(s => s.ToString()));
    }
}