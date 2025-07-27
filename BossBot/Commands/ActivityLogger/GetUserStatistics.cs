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
        List<StringBuilder> resultList = [];
        var result = new StringBuilder();
        resultList.Add(result);
        var maxUserNameLength = usersInfo.Max(u => u.UserName.Length) - 3;
        
        usersInfo.ForEach(u =>
        {
            
            var str = $"{StringHelper.PopulateWithWhiteSpaces(u.UserId.ToString(), 2)} | {u.UserName} | {u.Count}";
            if ((result.Length + str.Length) > 2000)
            {
                result = new StringBuilder();
                resultList.Add(result);
            }
            result.AppendLine(str);
        });
        return resultList.Select(r => r.ToString()).ToList();
    }
}