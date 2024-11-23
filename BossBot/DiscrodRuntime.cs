using Discord;
using Discord.WebSocket;
using System.Text;
using BossBot.Commands;
using BossBot.Interfaces;
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
        private readonly Logger _logger = new();
        private readonly ImageWork _imageWork;

        public DiscordRuntime(Options? options)
        {
            _options = options;
            _dateTimeHelper = new DateTimeHelper(_options.TimeZone);
            _bossData = new BossData(_options);
            _imageWork = new ImageWork(_bossData, _dateTimeHelper, _options);
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
                new ClearBossCommand(_bossData),
                new RestartTimeCommand(_bossData, _dateTimeHelper),
                new AdenBossCommand(_bossData, _dateTimeHelper),
                new OrenBossCommand(_bossData, _dateTimeHelper),
                new SetUserTimeZoneCommand(_bossData)
            });
        }

        private Task Client_LoggedIn() => _logger.WriteLog("LoggedIn");

        private Task Client_Log(LogMessage arg) => _logger.WriteLog(arg.ToString());

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
                            ProcessMessage(message, arg.Channel);
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
                                $"**{StringHelper.PopulateWithWhiteSpaces(item.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{item.NickName.ToUpper()}**| через {timeToRespawn.ToString(@"hh\:mm")} | {item.Chance} {BossUtils.GetChanceStatus(item.Chance)}{BossUtils.AppendEggPlant(item.PurpleDrop)}");
                        }

                        var channel = _client.GetChannel(i) as ITextChannel;
                        channel?.SendMessageAsync(builder.ToString());
                    }
                }

                Thread.Sleep(60 * 1000);
            }
        }

        private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel)
        {
            if (message.Attachments.Any())
            {
                var image = message.Attachments.First();
                if(image.ContentType.StartsWith("image/"))
                {
                    var result = await ProcessImage(image.Url, channel.Id, message.Author.Id);
                    ProcessAnswers(channel, [result]);
                    return;
                }
            }
            
            // if (channel.Name != _options.ChatName || message.Content == null)
            //     return;

            var content = message.Content;
            var lines = Regex.Split(content, "\r\n|\r|\n");
            var answers = new List<string>();
            foreach (var line in lines)
            {
                if(!line.StartsWith("!"))
                    continue;
                
                var messageParts = line.Remove(0, 1).Split(' ');
                if (messageParts.Any())
                {
                    var command = _commands.FirstOrDefault(c => c.Keys.Contains(messageParts[0].ToLower()));
                    if(command == null)
                        continue;
                    var result = await command.ExecuteAsync(channel.Id, message.Author.Id, messageParts);
                    answers.AddRange(result);
                }
            }

            if (answers.Count > 0)
            {
                ProcessAnswers(channel, answers);
            }
        }

        private Task ProcessAnswers(ISocketMessageChannel channel, List<string> answers)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var answer in answers)
            {
                if (builder.Length + answer.Length > 2000)
                {
                    channel.SendMessageAsync(builder.ToString());
                    builder.Clear();
                }

                builder.AppendLine(answer);
            }
            return channel.SendMessageAsync(builder.ToString());
        }

        private Task<string> ProcessImage(string url, ulong chatId, ulong usedId) =>
            _imageWork.ProcessImage(url, chatId, usedId);
    }
}