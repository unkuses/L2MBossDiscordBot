using BossBot.Interfaces;
using System.Text;
using CommonLib.Helpers;
using CommonLib.Models;

namespace BossBot.Commands
{
    public class RestartTimeCommand(CosmoDb bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } = ["r", "р"];

        public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
        {
            var list = new List<string>();
            if (commands.Length is 1 or > 3)
            {
                list.Add("Не правильный формат");
                return list.Select(s => s);
            }
            var time = commands.Length == 2 ? dateTimeHelper.ParseCommand(commands[1]) : dateTimeHelper.ParseCommand(commands[1], commands[2]);
            if (!time.HasValue)
            {
                list.Add("Не правильный формат");
                return list.Select(s => s);
            }

            await bossData.PredictedTimeAfterRestartAsync(chatId, time);
            return await GetBossInformation(chatId);
        }

        private async Task<IEnumerable<string>> GetBossInformation(ulong chatId)
        {
            var bosses = await bossData.GetAllLoggedBossInfoAsync(chatId);
            return await PopulateBossInformationString(bosses);
        }

        private Task<IEnumerable<string>> PopulateBossInformationString(IList<BossModel> models)
        {
            List<StringBuilder> builders = new();
            var stringBuilder = new StringBuilder();
            builders.Add(stringBuilder);
            int maxLength = models.Max(x => x.NickName.Length);
            foreach (var model in models)
            {
                var nextRespawnTime = model.KillTime.AddHours(model.RespawnTime);
                var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
                var str =
                    $@"**{StringHelper.PopulateWithWhiteSpaces(model.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{StringHelper.PopulateWithWhiteSpaces(model.NickName.ToUpper(), maxLength)}** через {timeToRespawn.ToString(@"hh\:mm")} | {model.Chance}";
                if (stringBuilder.Length + str.Length > 2000)
                {
                    stringBuilder = new StringBuilder();
                    builders.Add(stringBuilder);
                }

                stringBuilder.AppendLine(str);
            }

            return Task.FromResult(builders.Select(b => b.ToString()));
        }
    }
}