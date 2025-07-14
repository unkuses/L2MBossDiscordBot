using BossBot.Commands;
using BossBot.DBModel;
using BossBot.Interfaces;
using CommonLib.Helpers;
using CommonLib.Models;
using CommonLib.Requests;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using BossBot.Model;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private readonly List<IEventCommand> _eventCommands = [];
        private readonly Logger _logger = new();
        private readonly OpenAIService _openAiService;
        private readonly RegisterChatCommand _registerChatCommand;

        public DiscordRuntime(Options? options)
        {
            _options = options;
            _dateTimeHelper = new DateTimeHelper(_options.TimeZone);
            _cosmoDb = new CosmoDb(_dateTimeHelper, _options.CosmoDbUrl, _options.CosmoDbKey);
            _bossData = new BossData(_options, _dateTimeHelper);
            _openAiService = new OpenAIService(_options, _cosmoDb, _dateTimeHelper);
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
                new UnregisterChatCommand(_bossData, _cosmoDb)
            ]);
            _eventCommands.AddRange(
            [
                new AddEventCommand(_bossData, _dateTimeHelper),
                new RemoveEventCommand(_bossData),
                new GetAllEventsCommand(_bossData),
            ]);

            _registerChatCommand = new RegisterChatCommand(_bossData);
        }

        private async Task Client_LoggedIn() => await _logger.WriteLog("LoggedIn");

        private async Task Client_Log(LogMessage arg) => await _logger.WriteLog(arg.ToString());

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

        public async Task StartDailyJob()
        {
            await Task.Delay(1000 * 60);
            while (true)
            {
                var now = _dateTimeHelper.CurrentTime;
                var nextRun = now.Date.AddHours(9);
                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                await Task.Delay(delay);
                try
                {
                    await GetAllDailyEvents();
                }
                catch (Exception ex)
                {
                    await _logger.WriteLog($"Daily job error: {ex.Message}");
                }
            }
        }

        private async Task GetAllDailyEvents()
        {
            var events = _bossData.GetAllTodayEvents();
            Dictionary<ulong, IList<EventInformationDBModel>> dictionary = new();
            foreach (var e in events)
            {
                if (!dictionary.ContainsKey(e.ChatId))
                {
                    dictionary[e.ChatId] = new List<EventInformationDBModel>();
                }

                dictionary[e.ChatId].Add(e);
            }

            foreach (var chatId in dictionary.Keys)
            {
                var builder = new StringBuilder();
                builder.AppendLine("@here Ближайшие события:");
                foreach (var item in dictionary[chatId])
                {
                    var timeToEvent = item.Time - _dateTimeHelper.CurrentTime;
                    builder.AppendLine($"**{item.EventName}** в {item.Time:HH:mm} через {timeToEvent.ToString(@"hh\:mm")}");
                }
                var channel = _client.GetChannel(chatId) as ITextChannel;
                await channel?.SendMessageAsync(builder.ToString());
            }
        }

        public async Task MaintenanceTask()
        {
            while (true)
            {
                var postponeBosses = await _cosmoDb.GetAndUpdateAllPostponeBossesAsync();
                var appendBosses = await _cosmoDb.GetAllAppendingBossesAsync();
                var upcomingEvents = _bossData.GetAllEvents();
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
            var now = _dateTimeHelper.CurrentTime;
            var nowTime = new TimeSpan(now.Hour, now.Minute, 0);
            var eventTime = new TimeSpan(time.Hour, time.Minute, 0);

            // Calculate the difference in minutes
            return Convert.ToInt32((eventTime - nowTime).TotalMinutes);
        }

        private async Task ProcessMessage(IMessage message, ISocketMessageChannel channel, string timeZone)
        {
            if (message.Content == null)
                return;

            if (channel.Name == _options.ChatEvent)
            {
                await ProcessEventMessages(message, channel);
            }
            else if (_bossData.ChatIsRegistered(channel.Id))
            {
                await ProcessBossMessages(message, channel, timeZone);
            }
            else if (_registerChatCommand.Keys.Contains(message.Content.ToLower()))
            {
                var result = await _registerChatCommand.ExecuteAsync(channel.Id, message.Author.Id, [string.Empty]);
                _ = ProcessAnswers(channel, result.ToList());
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
                    _ = ProcessAnswers(channel, [result]);
                    return;
                }
            }

            if (message.MentionedUserIds.Contains(_client.CurrentUser.Id))
            {
                _ = OpenAiBossMessage(message, channel, message.Content);
                return;
            }

            var content = message.Content;
            var lines = Regex.Split(content, "\r\n|\r|\n");
            var answers = new List<string>();
            foreach (var line in lines)
            {
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
            if (message.MentionedUserIds.Contains(_client.CurrentUser.Id))
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
                await ProcessAnswers(channel, [.. result]);
            }
        }

        private async Task OpenAiBossMessage(IMessage message, ISocketMessageChannel channel, string text)
        {
            var result = await _openAiService.GetBossResponseAsync(message.Channel.Id, text);
            await ProcessAnswers(channel, [result]);
        }

        private async Task OpenAiEventMessage(IMessage message, ISocketMessageChannel channel, string text)
        {
            var commandText = await _openAiService.GetEventResponseAsync(text);

            var eventModel = JsonConvert.DeserializeObject<EventCommandModel>(commandText);
            var eventCommand = _eventCommands.FirstOrDefault(c => c.Keys.Contains(eventModel.Event.ToString().ToLower()));
            var result = await eventCommand.ExecuteAsync(channel.Id, message.Author.Id, commandText);
            await ProcessAnswers(channel, [.. result]);
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