using BossBot.Interfaces;
using System.Text;

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
            if (commands.Length is 1 or > 4)
            {
                sb.AppendLine("Не правильный формат");
            }
            else
            {
                if (!int.TryParse(commands[1], out var id))
                {
                    sb.AppendLine("Не правильный формат");
                }

                DateTime? dateTime = null;
                if (commands.Length == 2)
                {
                    dateTime = dateTimeHelper.CurrentTime;
                }
                else
                {
                    dateTime = commands.Length == 3
                        ? dateTimeHelper.ParseCommand(commands[2])
                        : dateTimeHelper.ParseCommand(commands[2], commands[3]);
                }

                if (!dateTime.HasValue)
                {
                    sb.AppendLine("Не правильный формат");
                }

                if (dateTime > dateTimeHelper.CurrentTime.AddHours(1))
                    dateTime = dateTime.Value.AddDays(-1);
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
    }
}