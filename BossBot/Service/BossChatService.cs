using BossBot.Commands;
using BossBot.Commands.BossInfo;
using BossBot.Interfaces;
using BossBot.Options;
using CommonLib.Requests;
using Discord;
using Discord.WebSocket;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BossBot.Service;

public class BossChatService
{
    private readonly List<ICommand> _commands = [];
    private readonly BotOptions _options;
    private readonly BossData _bossData;
    private readonly RegisterChatCommand _registerChatCommand;
    private readonly DiscordClientService _discordClientService;
    private readonly OpenAIService _openAiService;
    private readonly Logger _logger;
    private readonly ChatLanguageData _chatLanguageData;

    public BossChatService(BotOptions options,
        DiscordClientService discordClientService,
        BossData bossData,
        OpenAIService openAiService,
        ChatLanguageData chatLanguageData,
        Logger logger,
        ClearBossCommand clearBossCommand, 
        GetAllBossCommand getAllBossCommand, 
        GetAllBossInformationCommand getAllBossInformationCommand, 
        GetAllNotLoggedBossesCommand getAllNotLoggedBossesCommand,
        GetBossListWithKillTimeCommand getBossListWithKillTimeCommand,
        LogKillBossCommand logKillBossCommand,
        RestartTimeCommand restartTimeCommand,
        SetUserTimeZoneCommand setUserTimeZoneCommand,
        RegisterChatCommand registerChatCommand,
        UnregisterChatCommand unregisterChatCommand,
        ChatLanguageCommand chatLanguageCommand) 
    {
        _logger = logger;
        _options = options;
        _discordClientService = discordClientService;
        _bossData = bossData;
        _chatLanguageData = chatLanguageData;
        _openAiService = openAiService;
        _discordClientService.MessageReceivedEvent += async (sender, args) =>
        {
            var timeZone = bossData.GetUserTimeZone(args.Item1.Author.Id);
            await ProcessMessage(args.Item1, args.Item2, timeZone);
        };
        _commands.Add(clearBossCommand);
        _commands.Add(getAllBossCommand);
        _commands.Add(getAllBossInformationCommand);
        _commands.Add(getAllNotLoggedBossesCommand);
        _commands.Add(getBossListWithKillTimeCommand);
        _commands.Add(logKillBossCommand);
        _commands.Add(restartTimeCommand);
        _commands.Add(setUserTimeZoneCommand);
        _commands.Add(unregisterChatCommand);
        _commands.Add(chatLanguageCommand);
        _registerChatCommand = registerChatCommand;
    }

    private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel, string timeZone)
    {
        if (message.Content == null || channel.Name == "активность" || channel.Name == _options.ChatEvent || channel.Name.ToLower() == "активності".ToLower())
            return;

        if (_bossData.ChatIsRegistered(channel.Id))
        {
            await ProcessBossMessages(message, channel, timeZone);
        }
        else if (_registerChatCommand.Keys.Contains(message.Content.ToLower()))
        {
            var result = await _registerChatCommand.ExecuteAsync(channel.Id, message.Author.Id, [string.Empty]);
            _ = _discordClientService.ProcessAnswers(channel, result.ToList());
        }
    }

    private async Task ProcessBossMessages(IMessage message, ISocketMessageChannel channel, string timeZone)
    {

        if (message.Attachments.Any())
        {
            foreach (var attachment in message.Attachments)
            {
                if (attachment.ContentType.StartsWith("image/"))
                {
                    var result = await ProcessImage(attachment.Url, channel.Id, timeZone,
                        _chatLanguageData.GetLanguage(channel.Id));
                    _ = _discordClientService.ProcessAnswers(channel, [result]);
                    return;
                }
            }
        }

        if (message.MentionedUserIds.Contains(_discordClientService.CurrentUserId))
        {
            _ = OpenAiBossMessage(message, channel, message.Content);
            return;
        }

        var content = message.Content;
        var lines = Regex.Split(content, "\r\n|\r|\n");
        var answers = new List<string>();
        foreach (var line in lines)
        {
            if (!line.StartsWith('!'))
                continue;
            var messageParts = line.Remove(0, 1).Split(' ');
            if (messageParts.Any())
            {
                var command = _commands.FirstOrDefault(c => c.Keys.Contains(messageParts[0].ToLower()));
                if (command == null)
                    continue;
                var result = await command.ExecuteAsync(channel.Id, message.Author.Id, messageParts);
                answers.AddRange(result);
            }
        }

        if (answers.Count > 0)
        {
            await _discordClientService.ProcessAnswers(channel, answers);
        }
    }

    private async Task OpenAiBossMessage(IMessage message, ISocketMessageChannel channel, string text)
    {
        var result = await _openAiService.GetBossResponseAsync(message.Channel.Id, text);
        await _discordClientService.ProcessAnswers(channel, [result]);
    }

    private async Task<string> ProcessImage(string url, ulong chatId, string timeZone, string language)
    {
        try
        {
            var requestData = new RequestParseImageUrl
            {
                Url = url,
                ChatId = chatId,
                TimeZone = timeZone,
                Language = language
            };
            var jsonPayload = JsonSerializer.Serialize(requestData);

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(_options.ImageAnalysisUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.WriteLog($"Error processing image: {ex.Message}");
            return $"Error processing image: {ex.Message}";
        }
    }
}
