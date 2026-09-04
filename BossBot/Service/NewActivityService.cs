using BossBot.Commands.NewActivityLogger;
using BossBot.Interfaces;
using Discord;
using Discord.WebSocket;

namespace BossBot.Service;

public class NewActivityService
{
    private readonly DiscordClientService _discordClientService;
    private readonly List<ICommand> _commands = [];
    public NewActivityService(DiscordClientService discordClientService, 
        RegisterUsers registerUsers, 
        ClearUsers clearUsers, 
        RegisterActivity registerActivity,
        GetEventStatistic statistic)
    {
        _commands.Add(registerUsers);
        _commands.Add(clearUsers);
        _commands.Add(registerActivity);
        _commands.Add(statistic);

        _discordClientService = discordClientService;
        _discordClientService.MessageReceivedEvent += ProcessMessage;
    }

    private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel)
    {
        if (message.Content == null  || channel.Name != "активность")
            return;

        var image = message.Attachments.FirstOrDefault();

        var result = await CommandResponse(message.Content, channel.Id, message.Author.Id, image?.Url ?? string.Empty);
        _ = _discordClientService.ProcessAnswers(channel, result);
    }

    public async Task<List<string>> CommandResponse(string command, ulong chatId, ulong userId, string url)
    {
        if (string.IsNullOrEmpty(command))
            return ["Комманда не найдена"];

        var commandParts = command.Split(' ');
        var cmd = _commands.FirstOrDefault(c => c.Keys.Contains(commandParts[0], StringComparer.OrdinalIgnoreCase));
        return cmd != null
            ? await cmd.ExecuteAsync(chatId, userId, commandParts, url)
            : ["Комманда не найдена"];
    }
}
