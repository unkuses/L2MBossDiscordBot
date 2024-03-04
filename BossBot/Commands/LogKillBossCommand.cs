using BossBot.Interfaces;
using Discord.WebSocket;

namespace BossBot.Commands
{
    public class LogKillBossCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } = ["k", "к"];

        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            if (commands.Length < 2)
            {
                channel.SendMessageAsync("Не правильный формат");
            }
            else
            {
                if (!int.TryParse(commands[1], out var id))
                {
                    channel.SendMessageAsync("Не правильный формат");
                    return Task.CompletedTask;
                }

                var dateTime = ParseDateTimeParameters(commands);
                if (!dateTime.HasValue)
                {
                    channel.SendMessageAsync("Не правильный формат");
                    return Task.CompletedTask;
                }

                var boss = bossData.LogKillBossInformation(channel.Id, id, dateTime.Value);
                if (boss == null)
                {
                    return channel.SendMessageAsync($"Босс с номером {id} не был найден");
                }
                else
                {
                    var nextRespawnTime = boss.KillTime.AddHours(boss.RespawnTime);
                    var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;

                    var msg =
                        $"Босс убит **{boss.Id}** **{boss.NickName.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}";
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

                    if (dateTime > dateTimeHelper.CurrentTime.AddHours(1))
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
    }
}