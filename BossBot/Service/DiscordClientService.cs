using BossBot.Options;
using Discord;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Service;

public class DiscordClientService
{
    private readonly DiscordSocketClient _client;
    private readonly Logger _logger;
    private readonly Dictionary<ulong, DateTimeOffset> _lastReadMessage = new();
    public DiscordClientService(BotOptions options, Logger logger)
    {
        _client = new DiscordSocketClient();
        _logger = logger;

        _client.MessageReceived += MessageReceived;
        _client.Log += Client_Log;
        _client.LoggedIn += Client_LoggedIn;
        _ = LogIn(options.BotToken);
    }

    public event Func<IMessage, ISocketMessageChannel, Task>? MessageReceivedEvent;

    public async Task ProcessAnswers(ISocketMessageChannel channel, List<string> answers)
    {
        var builder = new StringBuilder();
        foreach (var answer in answers)
        {
            if (builder.Length + answer.Length > 2000)
            {
                await channel.SendMessageAsync(builder.ToString());
                builder.Clear();
            }

            builder.AppendLine(answer);
        }
        if (builder.Length > 0)
            await channel.SendMessageAsync(builder.ToString());
    }

    private async Task MessageReceived(SocketMessage arg)
    {
        var messages = await arg.Channel.GetMessagesAsync(10).ToListAsync();
        foreach (var message in messages.SelectMany(batch => batch).OrderBy(message => message.CreatedAt))
        {
            if (message.Author.IsBot)
                continue;

            if (_lastReadMessage.TryGetValue(arg.Channel.Id, out var lastRead) &&
                message.CreatedAt <= lastRead)
            {
                continue;
            }

            _ = DispatchMessage(message, arg.Channel);
            _lastReadMessage[arg.Channel.Id] = message.CreatedAt;
        }
    }

    private async Task DispatchMessage(IMessage message, ISocketMessageChannel channel)
    {
        if (MessageReceivedEvent == null)
            return;

        try
        {
            var tasks = MessageReceivedEvent.GetInvocationList()
                .Cast<Func<IMessage, ISocketMessageChannel, Task>>()
                .Select(handler => handler(message, channel));
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            await _logger.WriteLog($"Message handler failed: {ex}");
        }
    }

    private async Task Client_LoggedIn() => await _logger.WriteLog("LoggedIn");

    private async Task Client_Log(LogMessage arg) => await _logger.WriteLog(arg.ToString());

    public async Task LogIn(string botToken)
    {
        await _client.LoginAsync(TokenType.Bot, botToken);
        await _client.StartAsync();
    }

    public ITextChannel? GetChannel(ulong chatId) =>
        _client.GetChannel(chatId) as ITextChannel;
    
    public ulong CurrentUserId => _client.CurrentUser.Id;
}
