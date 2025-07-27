using BossBot.Interfaces;

namespace BossBot.Commands.ActivityLogger;

public class MergeUsersCommand(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["merge", "объединить", "m"];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (commands.Length < 3)
            return Task.FromResult<List<string>>(["Неверный формат команды. Используйте: merge <id1> <id2>"]);
        
        if (!int.TryParse(commands[1], out var firstUserId))
        {
            return Task.FromResult<List<string>>(["Неверный формат команды. Используйте: merge <id1> <id2>"]);
        }
        List<int> secondUserIds = [];
        for (var i = 2; i < commands.Length; ++i)
        {
            if (int.TryParse(commands[i], out var id))
            {
                secondUserIds.Add(id);
            }
        }

        if(secondUserIds.Count == 0)
        {
            return Task.FromResult<List<string>>(["Неверный формат команды. Используйте: merge <id1> <id2>"]);
        }

        var result = userStatisticData.MergeUsers(chatId, firstUserId, secondUserIds);

        return result ? Task.FromResult<List<string>>(["Пользователи успешно объединены"]) : Task.FromResult<List<string>>(["Не удалось объединить пользователей. Проверьте, что оба пользователя существуют и принадлежат этому чату."]);
    }
}