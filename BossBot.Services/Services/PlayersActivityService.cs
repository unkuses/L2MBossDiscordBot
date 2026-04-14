using System.Text.RegularExpressions;
using BossBot.Services.Database;
using CommonLib.Models;

namespace BossBot.Services.Services;

public class PlayersActivityService(DatabaseService databaseService)
{
    public async Task<List<string>> PopulatePlayersActivityAsync(ulong chatId, List<string> allLines, string eventName)
    {
        var playersNames = await databaseService.GetAllPlayersNamesAsync(chatId);

        var result = new List<string>();
        foreach (var line in allLines)
        {
            var cleaned = Regex.Replace(line, "[^a-zA-Zа-яА-ЯёЁ0-9]", "")
                .ToLower();
            var playerName = playersNames
                .FirstOrDefault(p => string.Equals(p, cleaned, StringComparison.CurrentCultureIgnoreCase));
            if (playerName != null && !result.Contains(playerName))
            {
                result.Add(playerName);
            }
        }
        await databaseService.AddPlayerActivitiesAsync(chatId, result, eventName);
        return result;
    }

    public Task<List<string>> AddNewPlayerName(ulong chatId, List<string> playerName) =>
        databaseService.AddNewPlayerNameAsync(chatId, playerName);
    

    public Task<List<EventStatistic>> GetUserStatisticByEventName(ulong chatId, string eventName) =>
        databaseService.GetUserStatisticByEventName(chatId, eventName);

    public Task CleanUsers(ulong chatId) =>
         databaseService.CleanUsersByChatId(chatId);
}