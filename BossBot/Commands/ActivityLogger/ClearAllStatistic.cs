using BossBot.Interfaces;

namespace BossBot.Commands.ActivityLogger;

public class ClearAllStatistic(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["c", "clear", "очистить", "о"];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        userStatisticData.ClearAllInformation(chatId);
        return Task.FromResult<List<string>>(["Вся статистика удалена"]);
    }
}