using BossBot.Interfaces;
using BossBot.Options;

namespace BossBot.Commands.NewActivityLogger;

public class ClearUsers(BotOptions options) : ICommand
{
    public string[] Keys { get; } = ["clear", "очистить" ];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands, string screenShotUrl = "")
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.DeleteAsync($"{options.BaseFunctionPath}CleanUsers/?chatId={chatId}{options.FunctionSecret}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return [result];
        }
        return ["Failed to register user. Please try again later."];
    }
}