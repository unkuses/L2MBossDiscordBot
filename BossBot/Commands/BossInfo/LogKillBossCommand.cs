using BossBot.Interfaces;
using System.Text;
using CommonLib.Helpers;

namespace BossBot.Commands.BossInfo
{
    public class LogKillBossCommand(CosmoDb bossData, DateTimeHelper dateTimeHelper, ILanguage localization) : ICommand
    {
        public string[] Keys { get; } = ["k", "к"];

        public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
        {
            List<StringBuilder> stringBuilders = [];
            var sb = new StringBuilder();
            stringBuilders.Add(sb);
            if (commands.Length is 1 or > 4)
            {
                sb.AppendLine(localization.IncorrectFormat(chatId));
            }
            else
            {
                var id = commands[1];

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
                    sb.AppendLine(localization.IncorrectFormat(chatId));
                }

                if (dateTime > dateTimeHelper.CurrentTime.AddHours(1))
                    dateTime = dateTime.Value.AddDays(-1);
                var boss = await bossData.LogKillBossInformationAsync(chatId, id, dateTime.Value);
                if (boss == null)
                {
                    sb.AppendLine(localization.BossNotFound(chatId, id));
                }
                else
                {
                    var nextRespawnTime = boss.KillTime.AddHours(boss.RespawnTime);
                    var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;

                    sb.AppendLine(localization.BossLogged(chatId, boss, nextRespawnTime, timeToRespawn));
                }
            }

            return stringBuilders.Select(s => s.ToString()).ToList();
        }
    }
}