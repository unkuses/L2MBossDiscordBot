using BossBot.Interfaces;

namespace BossBot.Commands.ActivityLogger;

public class RegisterUser(UserStatisticData userStatisticData) : ICommand
{
    public string[] Keys { get; } = ["reg", "рег"];
    public Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var userName = commands[1];
        var count = 1;
        if (commands.Length >= 2)
        {
            int.TryParse(commands[2], out count);
        }
        userStatisticData.CreateOrUpdateUser(chatId, userName, count);

        return Task.FromResult<List<string>>(["Информация о пользователе добавлена либо обновленна"]);
    }
}