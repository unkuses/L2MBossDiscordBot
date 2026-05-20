using BossBot.Interfaces;
using BossBot.Options;
using CommonLib.Requests;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BossBot.Commands.NewActivityLogger;

public class RegisterActivity(BotOptions options) : ICommand
{
    public string[] Keys { get; } = ["!осада", "!олимп", "!сбор"];

    public readonly List<string> EventList = ["осада", "олимп", "сбор"];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands, string screenShotUrl = "")
    {
        var eventName = commands[0].Remove(0,1);
        if(!EventList.Contains(eventName))
        {
            return ["Invalid event name. Please use one of the following: осада, олимп, сбор."];
        }

        RequestAddUserToEvent requestData = new() { ChatId = chatId, Url = screenShotUrl, EventName = eventName};
        var jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var jsonPayload = JsonSerializer.Serialize(requestData, jsonOptions);

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync($"{options.BaseFunctionPath}AddUserToEvents{options.FunctionSecret}", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return [result];
        }
        return ["Failed to register user. Please try again later."];
    }
}