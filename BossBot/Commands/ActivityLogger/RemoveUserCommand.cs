using BossBot.Interfaces;

namespace BossBot.Commands.ActivityLogger;

public class RemoveUserCommand(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["remove", "удалить", "r", "del", "d"];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (commands.Length == 2 && int.TryParse(commands[1], out var id))
        {
            var result = userStatisticData.RemoveUser(chatId, id);
            if(result)             {
                return Task.FromResult<List<string>>(["Пользователь удален"]);
            }
        }

        return Task.FromResult<List<string>>(["Пользователь не найден"]);
    }
}