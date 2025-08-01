using BossBot.Commands.Event;
using BossBot.Interfaces;
using BossBot.Model;
using BossBot.Options;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BossBot.Service;

public class EventChatService
{
    private readonly List<IEventCommand> _eventCommands = [];
    private readonly BotOptions _options;
    private readonly DiscordClientService _discordClientService;
    private readonly OpenAIService _openAiService;
    private readonly BossData _bossData;
    public EventChatService(BotOptions options,
        DiscordClientService discordClientService,
        BossData bossData,
        OpenAIService openAiService,
        AddEventCommand addEventCommand,
        RemoveEventCommand removeEventCommand, 
        GetAllEventsCommand getAllEventsCommand)
    {
        _options = options;
        _openAiService = openAiService;
        _bossData = bossData;
        _discordClientService = discordClientService;
        _discordClientService.MessageReceivedEvent += async (sender, args) =>
        {
            var timeZone = bossData.GetUserTimeZone(args.Item1.Author.Id);
            await ProcessMessage(args.Item1, args.Item2, timeZone);
        };
        _eventCommands.AddRange(
        [ addEventCommand, removeEventCommand, getAllEventsCommand ] );
    }

    private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel, string timeZone)
    {
        if (message.Content == null || channel.Name != _options.ChatEvent)
            return;

        await ProcessEventMessages(message, channel);
    }

    private async Task ProcessEventMessages(IMessage message, ISocketMessageChannel channel)
    {
        if (message.MentionedUserIds.Contains(_discordClientService.CurrentUserId))
        {
            await OpenAiEventMessage(message, channel, message.Content);
            return;
        }

        var messageParts = message.Content.Remove(0, 1).Split(' ');
        if (messageParts.Any())
        {
            var command = _eventCommands.FirstOrDefault(c => c.Keys.Contains(messageParts[0].ToLower()));
            if (command == null)
                return;
            var result = await command.ExecuteAsync(channel.Id, message.Author.Id, messageParts);
            await _discordClientService.ProcessAnswers(channel, [.. result]);
        }
    }

    private async Task OpenAiEventMessage(IMessage message, ISocketMessageChannel channel, string text)
    {
        var commandText = await _openAiService.GetEventResponseAsync(text);

        var eventModel = JsonConvert.DeserializeObject<EventCommandModel>(commandText);
        var eventCommand = _eventCommands.FirstOrDefault(c => c.Keys.Contains(eventModel.Event.ToString().ToLower()));
        var result = await eventCommand.ExecuteAsync(channel.Id, message.Author.Id, commandText);
        await _discordClientService.ProcessAnswers(channel, [.. result]);
    }
}