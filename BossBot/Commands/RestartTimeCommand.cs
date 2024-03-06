using BossBot.Interfaces;
using System.Text;

namespace BossBot.Commands
{
    public class RestartTimeCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } = ["r", "р"];

        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            var list = new List<string>();
            if (commands.Length is 1 or > 3)
            {
                list.Add("Не правильный формат");
                return Task.FromResult(list.Select(s => s));
            }
            var time = commands.Length == 2 ? dateTimeHelper.ParseCommand(commands[1]) : dateTimeHelper.ParseCommand(commands[1], commands[2]);
            if (!time.HasValue)
            {
                list.Add("Не правильный формат");
                return Task.FromResult(list.Select(s => s));
            }

            bossData.PredictedTimeAfterRestart(chatId, time.Value);
            return GetBossInformation(chatId);
        }

        private Task<IEnumerable<string>> GetBossInformation(ulong chatId)
        {
            var bosses = bossData.GetAllLoggedBossInfo(chatId);
            return PopulateBossInformationString(bosses);
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
                }

                stringBuilder.AppendLine(str);
            }

            return Task.FromResult(builders.Select(b => b.ToString()));
        }
    }
}