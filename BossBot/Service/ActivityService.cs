using BossBot.Commands.ActivityLogger;
using BossBot.Interfaces;

namespace BossBot.Service;

public class ActivityService
{
    private readonly List<ICommand> _commands = [];
    private readonly AddUserCommand _addUserCommand;
    private readonly UserStatisticData _userStatisticData = new ();

    public ActivityService(Options options)
    {
        _addUserCommand = new AddUserCommand(options, _userStatisticData);
        _commands.Add(new ClearAllStatistic(_userStatisticData));
        _commands.Add(new GetUserStatistics(_userStatisticData));
        _commands.Add(new RemoveUserCommand(_userStatisticData));
    }

    public List<string> CommandResponse(string command, ulong chatId, ulong userId)
    {
        if(string.IsNullOrEmpty(command))
            return ["Комманда не найдена"];

        var commandParts = command.Split(' ');
        var cmd = _commands.FirstOrDefault(c => c.Keys.Contains(commandParts[0]));
        return cmd != null ? cmd.ExecuteAsync(chatId, userId, commandParts).Result.ToList() : ["Комманда не найдена"];
    }

    public string AddUser(ulong chatId, string url)
    {
        _addUserCommand.ExecuteAsync(chatId, url).Wait();
        return "Статистика Обновлена";
    }
}