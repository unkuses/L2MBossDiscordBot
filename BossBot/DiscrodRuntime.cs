using Discord;
using Discord.WebSocket;
using System.Text;
using System.Text.Json;
using BossBot.Commands;
using BossBot.Interfaces;
using System.Text.RegularExpressions;
using CommonLib.Helpers;
using CommonLib.Models;
using CommonLib.Requests;

namespace BossBot
{
    public class DiscordRuntime
    {
        private readonly DiscordSocketClient _client;
        private readonly BossData _bossData;
        private readonly CosmoDb _cosmoDb;
        private readonly Options? _options;
        private readonly Dictionary<ulong, DateTimeOffset> _lastReadMessage = new();
        private readonly DateTimeHelper _dateTimeHelper;
        private readonly List<ICommand> _commands = [];
        private readonly List<ICommand> _eventCommands = [];
        private readonly Logger _logger = new();

        public DiscordRuntime(Options? options)
        {
            _options = options;
            _dateTimeHelper = new DateTimeHelper(_options.TimeZone);
            _cosmoDb = new CosmoDb(_dateTimeHelper, _options.CosmoDbUrl, _options.CosmoDbKey);
            _bossData = new BossData(_options);
            _client = new DiscordSocketClient();

            _client.MessageReceived += Discord_MessageReceived;
            _client.Log += Client_Log;
            _client.LoggedIn += Client_LoggedIn;
            _commands.AddRange(
            [
                new GetAllBossCommand(_cosmoDb, _dateTimeHelper),
                new GetBossListWithKillTimeCommand(_cosmoDb),
                new GetAllNotLoggedBossesCommand(_cosmoDb),
                new GetAllBossInformationCommand(_cosmoDb),
                new LogKillBossCommand(_cosmoDb, _dateTimeHelper),
                new ClearBossCommand(_cosmoDb),
                new RestartTimeCommand(_cosmoDb, _dateTimeHelper),
                new AdenBossCommand(_cosmoDb, _dateTimeHelper),
                new OrenBossCommand(_cosmoDb, _dateTimeHelper),
                new SetUserTimeZoneCommand(_bossData),
            ]);
            _eventCommands.AddRange(
            [
                new AddEventCommand(_bossData),
                new RemoveEventCommand(_bossData),
                new GetAllEventsCommand(_bossData),
            ]);
        }

        private async Task Client_LoggedIn() => Console.WriteLine("LoggedIn"); // _logger.WriteLog("LoggedIn");

        private async Task Client_Log(LogMessage arg) => Console.WriteLine(arg.ToString());//_logger.WriteLog(arg.ToString());

        public async Task LogIn()
        {
            await _client.LoginAsync(TokenType.Bot, _options.BotToken);
            await _client.StartAsync();
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
                            _ = ProcessMessage(message, arg.Channel, _bossData.GetUserTimeZone(message.Author.Id));
                        }

                        _lastReadMessage[arg.Channel.Id] = message.CreatedAt;
                    }
                });
            }
        }

        public async Task MaintenanceTask()
        {
            while (true)
            {
                var postponeBosses = new List<BossModel>();//await _cosmoDb.GetAndUpdateAllPostponeBossesAsync());
                var appendBosses = new List<BossModel>(); //await _cosmoDb.GetAllAppendingBossesAsync();
                var upcomingEvents = _bossData.GetAllEvents();
                // var bossesToMention = await _cosmoDb.GetAllNotAnnouncedBossesAsync();
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

                if (upcomingEvents.Any())
                {
                    foreach (var upcomingEvent in upcomingEvents)
                    {
                        var channel = _client.GetChannel(upcomingEvent.ChatId) as ITextChannel;

                        channel?.SendMessageAsync(
                            $"@here **{upcomingEvent.EventName}** в {upcomingEvent.Time:HH:mm} через {TimeDifference(upcomingEvent.Time)} минут.");
                    }
                }

                Thread.Sleep(60 * 1000);
            }
        }

        private int TimeDifference(DateTime time)
        {
            var now = DateTime.Now;
            var nowTime = new TimeSpan(now.Hour, now.Minute, 0);
            var eventTime = new TimeSpan(time.Hour, time.Minute, 0);

            // Calculate the difference in minutes
            return Convert.ToInt32((eventTime - nowTime).TotalMinutes);
        }

        private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel, string timeZone)
        {
            if (message.Content == null)
                return;

            if(channel.Name == _options.ChatName)
            {
                await ProcessBossMessages(message, channel, timeZone);
            }
            else if (channel.Name == _options.ChatEvent)
            {
                await ProcessEventMessages(message, channel);
            }
        }

        private async Task ProcessBossMessages(IMessage message, ISocketMessageChannel channel, string timeZone)
        {

            if (message.Attachments.Any())
            {
                var image = message.Attachments.First();
                if (image.ContentType.StartsWith("image/"))
                {
                    var result = await ProcessImage(image.Url, channel.Id, timeZone);
                    ProcessAnswers(channel, [result]);
                    return;
                }
            }

            var content = message.Content;
            var lines = Regex.Split(content, "\r\n|\r|\n");
            var answers = new List<string>();
            foreach (var line in lines)
            {
                if (!line.StartsWith("!"))
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
                await ProcessAnswers(channel, answers);
            }
        }

        private async Task ProcessEventMessages(IMessage message, ISocketMessageChannel channel)
        {
            var content = message.Content;
            if (!content.StartsWith("!"))
                return;

            var messageParts = content.Remove(0, 1).Split(' ');
            if (messageParts.Any())
            {
                var command = _eventCommands.FirstOrDefault(c => c.Keys.Contains(messageParts[0].ToLower()));
                if (command == null)
                    return;
                var result = await command.ExecuteAsync(channel.Id, message.Author.Id, messageParts);
                await ProcessAnswers(channel, [.. result]);
            }
        }

        private Task ProcessAnswers(ISocketMessageChannel channel, List<string> answers)
        {
            var builder = new StringBuilder();
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

        private async Task<string> ProcessImage(string url, ulong chatId, string timeZone)
        {
            try
            {
                var requestData = new RequestParseImageUrl
                {
                    Url = url,
                    ChatId = chatId,
                    TimeZone = timeZone
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
}