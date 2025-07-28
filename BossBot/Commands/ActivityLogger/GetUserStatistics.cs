using BossBot.Interfaces;
using System.Text;

namespace BossBot.Commands.ActivityLogger;

public class GetUserStatistics(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["all", "все"];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var usersInfo = userStatisticData.GetUserStatistics(chatId);
        List<StringBuilder> resultList = [];
        var result = new StringBuilder();
        resultList.Add(result);

        // Calculate maxUserNameLength (at least 4 to avoid negative/too small)
        var maxUserNameLength = Math.Max(4, usersInfo.Any() ? usersInfo.Max(u => u.UserName.Length) : 4);

        // Table header
        var header = $"{"Id",-2} | {"Name".PadRight(maxUserNameLength)} | {"Count",-5}";
        var separator = $"{new string('-', 2)}-+-{new string('-', maxUserNameLength)}-+-{new string('-', 5)}";
        result.AppendLine("```");
        result.AppendLine(header);
        result.AppendLine(separator);

        foreach (var u in usersInfo)
        {
            var idStr = u.UserId.ToString().PadRight(2).Substring(0, 2);
            var nameStr = u.UserName.PadRight(maxUserNameLength).Substring(0, maxUserNameLength);
            var countStr = u.Count.ToString().PadRight(5).Substring(0, 5);

            var str = $"{idStr} | {nameStr} | {countStr}";
            if ((result.Length + str.Length) > 1900)
            {
                result.AppendLine("```");
                result = new StringBuilder();
                result.Append("```");
                resultList.Add(result);
            }
            result.AppendLine(str);
        }
        result.AppendLine("```");
        return resultList.Select(r => r.ToString()).ToList();
    }
}