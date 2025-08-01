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

    public event EventHandler<Tuple<IMessage, ISocketMessageChannel>> MessageReceivedEvent;

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
        await channel.SendMessageAsync(builder.ToString());
    }

    private async Task MessageReceived(SocketMessage arg)
    {
        var messages = await arg.Channel.GetMessagesAsync(10).ToListAsync();
        if (messages != null)
        {
            messages.ForEach(x =>
            {
                foreach (var message in x)
                {
                    if (!message.Author.IsBot)
                    {
                        if (_lastReadMessage.ContainsKey(arg.Channel.Id) &&
                            message.CreatedAt <= _lastReadMessage[arg.Channel.Id]) continue;
                        MessageReceivedEvent?.Invoke(this, new Tuple<IMessage, ISocketMessageChannel>(message, arg.Channel));
                    }

                    _lastReadMessage[arg.Channel.Id] = message.CreatedAt;
                }
            });
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