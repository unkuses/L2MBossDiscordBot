using BossBot.Interfaces;
using BossBot.Options;
using CommonLib.Requests;
using System.Text;
using System.Text.Json;

namespace BossBot.Commands.NewActivityLogger
{
    public class RegisterUsers(BotOptions options) : ICommand
    {
        public string[] Keys { get; } = ["reg", "рег"];
        public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands, string screenShotUrl = "")
        {
            RequestAddUser requestData = new () { ChatId = chatId, Url = screenShotUrl };
            var jsonPayload = JsonSerializer.Serialize(requestData);

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"{options.BaseFunctionPath}AddUsers{options.FunctionSecret}", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return [result];
            }
            return ["Failed to register user. Please try again later."];
        }
    }
}
