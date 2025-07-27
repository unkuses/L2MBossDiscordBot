using CommonLib.Requests;
using System.Text;
using System.Text.Json;

namespace BossBot.Commands.ActivityLogger;

public class AddUserCommand(Options options, UserStatisticData userStatisticData)
{
    public async Task ExecuteAsync(ulong chatId, string url)
    {
        var requestData = new RequestParseImageUrl
        {
            Url = url,
        };
        var jsonPayload = JsonSerializer.Serialize(requestData);

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(options.ImageStatisticAnalysisUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<List<string>>(responseString);
            responseData.ForEach(user => userStatisticData.AddUserStatistic(chatId, user));
        }
    }
}