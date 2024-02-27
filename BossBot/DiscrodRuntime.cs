﻿using System.Runtime.CompilerServices;
using Discord;
using Discord.WebSocket;
using System.Text;

namespace BossBot
{
    public class DiscordRuntime
    {
        private readonly DiscordSocketClient _client;
        private readonly BossData _bossData = new();
        private readonly Options _options;
        private readonly Dictionary<ulong, DateTimeOffset> _lastReadMessage = new();
        public DiscordRuntime(Options options)
        {
            _options = options;
            
            _client = new DiscordSocketClient();
            
            _client.MessageReceived += Discord_MessageReceived;
            _client.Log += Client_Log;
            _client.LoggedIn += Client_LoggedIn;
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

        public async Task LogIn()
        {
            await _client.LoginAsync(TokenType.Bot, _options.BotToken);
            await _client.StartAsync();
            MaintenanceTask();
        }

        private async Task Discord_MessageReceived(SocketMessage arg)
        {
            var messages = await arg.Channel.GetMessagesAsync(10).ToListAsync();
            if(messages != null)
            {
                messages.ForEach(x =>
                {
                    foreach (var message in x)
                    {
                        if (_lastReadMessage.ContainsKey(arg.Channel.Id) &&
                            message.CreatedAt <= _lastReadMessage[arg.Channel.Id]) continue;
                        ProcessMessage(message.Content, arg.Channel);
                        _lastReadMessage[arg.Channel.Id] = message.CreatedAt;
                    }
                });
            }
        }

        private Task MaintenanceTask()
        {
            while(true)
            {
                var postponeBosses = _bossData.GetAndUpdateAllPostponeBosses();
                var appendBosses = _bossData.GetAllAppendingBosses();
                if(postponeBosses.Count > 0)
                {
                    Dictionary<ulong, IList<BossModel>> dic = new();
                    foreach (var postponeBoss in postponeBosses)
                    {
                        if(!dic.ContainsKey(postponeBoss.ChatId.Value))
                        {
                            dic[postponeBoss.ChatId.Value] = new List<BossModel>();
                        }
                        dic[postponeBoss.ChatId.Value].Add(postponeBoss);
                    }
                    foreach(var i in dic.Keys)
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (var item in dic[i])
                        {
                            var nextRespawnTime = item.KillTime.AddHours(item.RespawnTime);
                            var timeToRespawn = nextRespawnTime - DateTimeMsc.CurrentTimeMcs;
                            builder.AppendLine($"Босс **{StringHelper.PopulateWithWhiteSpaces(item.Id.ToString(), 2)}** **{item.NickName.ToUpper()}** не был залогирован. Новое время {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}");
                        }
                        var channel = _client.GetChannel(i) as ITextChannel;
                        channel?.SendMessageAsync(builder.ToString());
                    }
                }
                if(appendBosses.Count > 0)
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
                            var timeToRespawn = nextRespawnTime - DateTimeMsc.CurrentTimeMcs;
                            builder.AppendLine($"**{StringHelper.PopulateWithWhiteSpaces(item.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{item.NickName.ToUpper()}**| через {timeToRespawn.ToString(@"hh\:mm")}");
                        }
                        var channel = _client.GetChannel(i) as ITextChannel;
                        channel?.SendMessageAsync(builder.ToString());
                    }
                }
                Thread.Sleep(60*1000);
            }
        }

        private Task ProcessMessage(string? message, ISocketMessageChannel channel)
        {
            if (channel.Name != _options.ChatName || message == null || !message.StartsWith("!"))
                return Task.CompletedTask;
            
            var messageParts = message.Remove(0, 1).Split(" ");
            if(messageParts.Any())
            {
                switch(messageParts[0])
                {
                    case "?":
                        GetAllBossInformation(channel); break;
                    case "??":
                        GetAllNotLoggedBosses(channel); break;
                    case "l":
                    case "л":
                        
                        if (messageParts.Length == 1 || !int.TryParse(messageParts[1], out var count))
                        {
                            GetBossInformation(channel);
                        }
                        else
                        {
                            GetFirstLoggedBossInfo(channel, count);
                        }
                        break;
                    case "k":
                    case "к":
                        LogKillBossTime(channel, messageParts);
                        break;
                    case "kl":
                        GetBossListWithKillTime(channel);
                        break;
                    default:
                        break;
                              
                }
            }
            
            return Task.CompletedTask;
        }

        private Task GetAllBossInformation(ISocketMessageChannel channel)
        {
            var list = _bossData.GetBossesInformation();
            StringBuilder str = new StringBuilder();
            foreach (var item in list)
            {
                str.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }
            return channel.SendMessageAsync(str.ToString());
        }

        private Task GetAllNotLoggedBosses(ISocketMessageChannel channel)
        {
            var list = _bossData.GetAllNotLoggedBossInformation(channel.Id);
            var str = new StringBuilder();
            foreach (var item in list)
            {
                str.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }
            channel.SendMessageAsync(str.ToString());
            return Task.CompletedTask;
        }

        private Task GetBossInformation(ISocketMessageChannel channel)
        {
            var bosses = _bossData.GetAllLoggedBossInfo(channel.Id);
            return channel.SendMessageAsync(PopulateBossInformationString(bosses));
        }

        private Task GetFirstLoggedBossInfo(ISocketMessageChannel channel, int count)
        {
            
            if (count <= 0)
            {
                return Task.CompletedTask;
            }
            var bosses = _bossData.GetFirstLoggedBossInfo(channel.Id, count);
            return channel.SendMessageAsync(PopulateBossInformationString(bosses));
        }

        private Task GetBossListWithKillTime(ISocketMessageChannel channel)
        {
            
            var bossListWithKillTime = GetAllBossStatus(_bossData.GetAllLoggedBossInfo(channel.Id));
            return channel.SendMessageAsync(bossListWithKillTime);
        }

        private Task LogKillBossTime(ISocketMessageChannel channel, string[] message)
        {
            if(message.Length < 2) 
            {
                channel.SendMessageAsync("Не правильный формат");
            }
            else
            {
                if (!int.TryParse(message[1], out var id))
                {
                    channel.SendMessageAsync("Не правильный формат");
                    return Task.CompletedTask;
                }
                var dateTime = ParseDateTimeParameters(message);
                if (!dateTime.HasValue)
                {
                    channel.SendMessageAsync("Не правильный формат");
                    return Task.CompletedTask;
                }

                var boss = _bossData.LogKillBossInformation(channel.Id, id, dateTime.Value);
                if(boss == null)
                {
                    return channel.SendMessageAsync($"Босс с номером {id} не был найден");
                }
                else
                {
                    var nextRespawnTime = boss.KillTime.AddHours(boss.RespawnTime);
                    var timeToRespawn = nextRespawnTime - DateTimeMsc.CurrentTimeMcs;
                    
                    var msg = $"Босс убит **{boss.Id}** **{boss.NickName.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}";
                    return channel.SendMessageAsync(msg);
                }
            }
            return Task.CompletedTask;
        }

        private DateTime? ParseDateTimeParameters(string[] parameters)
        {
            switch (parameters.Length)
            {
                case 2:
                    return DateTime.Now;
                case 3:
                    if (!DateTime.TryParse(parameters[2], out var dateTime))
                    {
                        return null;
                    }

                    if (dateTime > DateTimeMsc.CurrentTimeMcs.AddHours(1))
                        dateTime = dateTime.AddDays(-1);
                    return dateTime;
                case 4:
                    if (!DateTime.TryParse($"{parameters[2]} {parameters[3]}", out dateTime))
                    {
                        return null;
                    }
                    return dateTime;
                default:
                    return null;
            }
        }

        private string PopulateBossInformationString(IList<BossModel> models)
        {
            StringBuilder builder = new ();
            int maxLength = models.Max(x => x.NickName.Length);
            foreach (var model in models)
            {
                var nextRespawnTime = model.KillTime.AddHours(model.RespawnTime);
                var timeToRespawn = nextRespawnTime - DateTimeMsc.CurrentTimeMcs;
                builder.AppendLine($"**{StringHelper.PopulateWithWhiteSpaces(model.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{StringHelper.PopulateWithWhiteSpaces(model.NickName.ToUpper(), maxLength)}** через {timeToRespawn.ToString(@"hh\:mm")}");
            }

            return builder.ToString();
        }

        private string GetAllBossStatus(IList<BossModel> models)
        {
            StringBuilder statusBuilder = new StringBuilder();
            foreach (var model in models)
            {
                statusBuilder.AppendLine($"!k {model.Id} {model.KillTime:yyyy-MM-dd HH:mm}");
            }
            return statusBuilder.ToString();
        }
    }
}
