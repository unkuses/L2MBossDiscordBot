using System.Text;
using BossBot.Interfaces;

namespace BossBot.Commands.ActivityLogger;

public class GetUserStatistics(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["all", "все"];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var usersInfo = userStatisticData.GetUserStatistics(chatId);
        var result = new StringBuilder();
        usersInfo.ForEach(u => result.AppendLine($"{u.UserId} | {u.UserName} | {u.Count}"));
        return [result.ToString()];
    }
}