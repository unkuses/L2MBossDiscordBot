using BossBot.Commands.ActivityLogger;
using BossBot.Interfaces;
using BossBot.Options;
using Discord;
using Discord.WebSocket;

namespace BossBot.Service;

public class ActivityService
{
    private readonly List<ICommand> _commands = [];
    private readonly AddUserCommand _addUserCommand;
    private readonly UserStatisticData _userStatisticData;
    private readonly DiscordClientService _discordClientService;

    public ActivityService(BotOptions options, 
        DiscordClientService discordClientService,
        UserStatisticData userStatisticData,
        AddUserCommand addUserCommand,
        ClearAllStatistic clearAllStatistic,
        GetUserStatistics getUserStatistics,
        RemoveUserCommand removeUserCommand,
        RegisterUser registerUser,
        MergeUsersCommand mergeUsersCommand)
    {
        _discordClientService = discordClientService;
        _discordClientService.MessageReceivedEvent += async (sender, args) =>
        {
            await ProcessMessage(args.Item1, args.Item2);
        };

        _addUserCommand = addUserCommand;
        _userStatisticData = userStatisticData;
        _commands.Add(clearAllStatistic);
        _commands.Add(getUserStatistics);
        _commands.Add(removeUserCommand);
        _commands.Add(registerUser);
        _commands.Add(mergeUsersCommand);
    }

    private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel)
    {
        if (message.Content == null || channel.Name != "активность")
            return;

        if (message.Attachments.Any())
        {
            var image = message.Attachments.First();
            if (image.ContentType.StartsWith("image/"))
            {
                var result = await AddUser(channel.Id, image.Url);
                _ = _discordClientService.ProcessAnswers(channel, result);
                return;
            }
        }
        else
        {
            var result = CommandResponse(message.Content, channel.Id, message.Author.Id);
            _ = _discordClientService.ProcessAnswers(channel, result);
            return;
        }
    }

    public List<string> CommandResponse(string command, ulong chatId, ulong userId)
    {
        if(string.IsNullOrEmpty(command))
            return ["Комманда не найдена"];

        var commandParts = command.Split(' ');
        var cmd = _commands.FirstOrDefault(c => c.Keys.Contains(commandParts[0]));
        return cmd != null ? cmd.ExecuteAsync(chatId, userId, commandParts).Result.ToList() : ["Комманда не найдена"];
    }

    public Task<List<string>> AddUser(ulong chatId, string url) 
        => _addUserCommand.ExecuteAsync(chatId, url);
}