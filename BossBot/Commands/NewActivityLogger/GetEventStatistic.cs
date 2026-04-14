using BossBot.Interfaces;
using BossBot.Options;
using CommonLib.Models;
using CommonLib.Requests;
using System.Text;
using System.Text.Json;

namespace BossBot.Commands.NewActivityLogger;

public class GetEventStatistic(BotOptions options) : ICommand
{
    public string[] Keys { get; } = ["осада", "олимп", "сбор"];
    public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands, string screenShotUrl = "")
    {
        RequestGetStatistic requestData = new() { ChatId = chatId, EventName = commands[0] };
        var jsonPayload = JsonSerializer.Serialize(requestData);

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync($"{options.BaseFunctionPath}GetStatisticForEvent{options.FunctionSecret}", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var list = JsonSerializer.Deserialize<List<EventStatistic>>(responseJson, options);
            var usersInfo = list.OrderByDescending(x => x.Count)
                .ThenBy(x => x.PlayerName)
                .ToList();
            List<StringBuilder> resultList = [];
            var result = new StringBuilder();
            resultList.Add(result);

            var maxNameLength = Math.Max(
                4,
                usersInfo.Any() ? usersInfo.Max(u => (u.PlayerName ?? string.Empty).Length) : 4);

            var header = $"{"Id",-3} | {"Name".PadRight(maxNameLength)} | {"Count",-5}";
            var separator = $"{new string('-', 3)}-+-{new string('-', maxNameLength)}-+-{new string('-', 5)}";

            result.AppendLine("```");
            result.AppendLine(header);
            result.AppendLine(separator);

            for (int i = 0; i < usersInfo.Count; i++)
            {
                var u = usersInfo[i];

                var idStr = (i + 1).ToString().PadRight(3).Substring(0, 3);
                var name = u.PlayerName;
                var nameStr = name.PadRight(maxNameLength).Substring(0, maxNameLength);
                var countStr = u.Count.ToString().PadRight(5).Substring(0, 5);

                var line = $"{idStr} | {nameStr} | {countStr}";

                if (result.Length + line.Length + 4 > 1900)
                {
                    result.AppendLine("```");
                    result = new StringBuilder();
                    resultList.Add(result);

                    result.AppendLine("```");
                    result.AppendLine(header);
                    result.AppendLine(separator);
                }

                result.AppendLine(line);
            }

            result.AppendLine("```");

            return resultList.Select(r => r.ToString()).ToList();
        }
        return ["Failed to register user. Please try again later."];
    }
}