using System.Text;
using BossBot.Interfaces;
using Discord.WebSocket;

namespace BossBot.Commands
{
    public class LogKillBossCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } = ["k", "к"];

        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            List<StringBuilder> stringBuilders = new List<StringBuilder>();
            StringBuilder sb = new StringBuilder();
            stringBuilders.Add(sb);
            if (commands.Length < 2)
            {
                sb.AppendLine("Не правильный формат");
            }
            else
            {
                if (!int.TryParse(commands[1], out var id))
                {
                    sb.AppendLine("Не правильный формат");
                }

                var dateTime = ParseDateTimeParameters(commands);
                if (!dateTime.HasValue)
                {
                    sb.AppendLine("Не правильный формат");
                }

                var boss = bossData.LogKillBossInformation(chatId, id, dateTime.Value);
                if (boss == null)
                {
                    sb.AppendLine($"Босс с номером {id} не был найден");
                }
                else
                {
                    var nextRespawnTime = boss.KillTime.AddHours(boss.RespawnTime);
                    var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;

                    sb.AppendLine(
                        $"Босс убит **{boss.Id}** **{boss.NickName.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}");
                }
            }

            return Task.FromResult(stringBuilders.Select(s => s.ToString()));
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