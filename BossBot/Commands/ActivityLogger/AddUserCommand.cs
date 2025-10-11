using CommonLib.Requests;
using System.Text;
using System.Text.Json;
using BossBot.DBModel;
using BossBot.Options;
using BossBot.Utils;

namespace BossBot.Commands.ActivityLogger;

public class AddUserCommand(BotOptions options, UserStatisticData userStatisticData)
{
    public async Task<List<string>> ExecuteAsync(ulong chatId, string url)
    {
        var requestData = new RequestParseImageUrl
        {
            Url = url,
        };
        var jsonPayload = JsonSerializer.Serialize(requestData);

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(options.ImageStatisticAnalysisUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        var userList = new List<UserStatisticDBModel>();
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<List<string>>(responseString);
            responseData.ForEach(userName =>
            {
                var user = userStatisticData.AddUserStatistic(chatId, userName.Replace(" ", ""));
                if(user != null)
                {
                    userList.Add(user);
                }
            });
        }

        if (userList.Count == 0)
        {
            return ["No users found in the image."];
        }
        return StatisticUtils.FormatUserStatistics(userList);
    }
}