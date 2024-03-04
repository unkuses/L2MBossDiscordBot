using Discord;
using Discord.WebSocket;
using System.Text;
using BossBot.Commands;
using BossBot.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace BossBot
{
    public class DiscordRuntime
    {
        private readonly DiscordSocketClient _client;
        private readonly BossData _bossData;
        private readonly Options? _options;
        private readonly Dictionary<ulong, DateTimeOffset> _lastReadMessage = new();
        private readonly DateTimeHelper _dateTimeHelper;
        private readonly List<ICommand> _commands = [];

        public DiscordRuntime(Options? options)
        {
            _options = options;
            _dateTimeHelper = new DateTimeHelper(_options.TimeZone);
            _bossData = new BossData(_options);
            _client = new DiscordSocketClient();

            _client.MessageReceived += Discord_MessageReceived;
            _client.Log += Client_Log;
            _client.LoggedIn += Client_LoggedIn;
            _commands.AddRange(new ICommand[]
            {
                new GetAllBossCommand(_bossData, _dateTimeHelper),
                new GetBossListWithKillTimeCommand(_bossData),
                new GetAllNotLoggedBossesCommand(_bossData),
                new GetAllBossInformationCommand(_bossData),
                new LogKillBossCommand(_bossData, _dateTimeHelper),
                new HelpCommand(),
                new ClearBossCommand(_bossData),
                new RestartTimeCommand(_bossData, _dateTimeHelper)
            });
        }

        private Task Client_LoggedIn()
        {
            Console.WriteLine("LoggedIn");
            return Task.CompletedTask;
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }

        private void Test()
        {
            var info = _bossData.GetBossesInformation();
        }

        public async Task LogIn()
        {
            await _client.LoginAsync(TokenType.Bot, _options.BotToken);
            await _client.StartAsync();
            MaintenanceTask();
        }

        private async Task Discord_MessageReceived(SocketMessage arg)
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
                            ProcessMessage(message.Content, arg.Channel);
                        }

                        _lastReadMessage[arg.Channel.Id] = message.CreatedAt;
                    }
                });
            }
        }

        private Task MaintenanceTask()
        {
            while (true)
            {
                var postponeBosses = _bossData.GetAndUpdateAllPostponeBosses();
                var appendBosses = _bossData.GetAllAppendingBosses();
                if (postponeBosses.Count > 0)
                {
                    Dictionary<ulong, IList<BossModel>> dic = new();
                    foreach (var postponeBoss in postponeBosses)
                    {
                        if (!dic.ContainsKey(postponeBoss.ChatId.Value))
                        {
                            dic[postponeBoss.ChatId.Value] = new List<BossModel>();
                        }

                        dic[postponeBoss.ChatId.Value].Add(postponeBoss);
                    }

                    foreach (var i in dic.Keys)
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (var item in dic[i])
                        {
                            var nextRespawnTime = item.KillTime.AddHours(item.RespawnTime);
                            var timeToRespawn = nextRespawnTime - _dateTimeHelper.CurrentTime;
                            builder.AppendLine(
                                $"Босс **{StringHelper.PopulateWithWhiteSpaces(item.Id.ToString(), 2)}** **{item.NickName.ToUpper()}** не был залогирован. Новое время {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}");
                        }

                        var channel = _client.GetChannel(i) as ITextChannel;
                        channel?.SendMessageAsync(builder.ToString());
                    }
                }

                if (appendBosses.Count > 0)
                {
                    Dictionary<ulong, IList<BossModel>> dictionary = new();
                    foreach (var appendBoss in appendBosses)
                    {
                        if (!dictionary.ContainsKey(appendBoss.ChatId.Value))
                        {
                            dictionary[appendBoss.ChatId.Value] = new List<BossModel>();
                        }

                        dictionary[appendBoss.ChatId.Value].Add(appendBoss);
                    }

                    foreach (var i in dictionary.Keys)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("@here Ближайшие боссы");
                        foreach (var item in dictionary[i])
                        {
                            var nextRespawnTime = item.KillTime.AddHours(item.RespawnTime);
                            var timeToRespawn = nextRespawnTime - _dateTimeHelper.CurrentTime;
                            builder.AppendLine(
                                $"**{StringHelper.PopulateWithWhiteSpaces(item.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{item.NickName.ToUpper()}**| через {timeToRespawn.ToString(@"hh\:mm")} | {item.Chance}");
                        }

                        var channel = _client.GetChannel(i) as ITextChannel;
                        channel?.SendMessageAsync(builder.ToString());
                    }
                }

                Thread.Sleep(60 * 1000);
            }
        }

        private Task ProcessMessage(string? message, ISocketMessageChannel channel)
        {
            if (channel.Name != _options.ChatName || message == null)
                return Task.CompletedTask;

            var lines = Regex.Split(message, "\r\n|\r|\n");
            foreach (var line in lines)
            {
                if(!line.StartsWith("!"))
                    continue;
                
                var messageParts = line.Remove(0, 1).Split(' ');
                if (messageParts.Any())
                {
                    _commands.FirstOrDefault(c => c.Keys.Contains(messageParts[0]))
                        ?.ExecuteAsync(channel, messageParts);
                }
            }

            return Task.CompletedTask;
        }
    }
}