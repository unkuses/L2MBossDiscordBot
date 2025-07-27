using BossBot.Interfaces;
using CommonLib.Helpers;
using System.Text;

namespace BossBot.Commands.ActivityLogger;

public class GetUserStatistics(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["all", "все"];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var usersInfo = userStatisticData.GetUserStatistics(chatId);
        var result = new StringBuilder();
        var maxUserNameLength = usersInfo.Max(u => u.UserName.Length) - 3;
        usersInfo.ForEach(u => result.AppendLine($"{StringHelper.PopulateWithWhiteSpaces(u.UserId.ToString(), 2)} | {StringHelper.PopulateWithWhiteSpaces(u.UserName, maxUserNameLength)} | {u.Count}"));
        return [result.ToString()];
    }
}