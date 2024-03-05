using BossBot.Interfaces;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Commands
{
    public class GetAllBossCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } =
        [
            "l", "л"
        ]; 
        
        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            if (commands.Length == 1 || !int.TryParse(commands[1], out var count))
            {
                return GetBossInformation(chatId);
            }
            else
            {
                return GetFirstLoggedBossInfo(chatId, count);
            }
        }

        private Task<IEnumerable<string>> GetBossInformation(ulong chatId)
        {
            var bosses = bossData.GetAllLoggedBossInfo(chatId);
            return PopulateBossInformationString(bosses);
        }

        private Task<IEnumerable<string>> GetFirstLoggedBossInfo(ulong chatId, int count)
        {
            if (count <= 0)
            {
                return Task.FromResult(Enumerable.Empty<string>());
            }

            var bosses = bossData.GetFirstLoggedBossInfo(chatId, count);
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
                var str = $@"**{StringHelper.PopulateWithWhiteSpaces(model.Id.ToString(), 2)}**|{nextRespawnTime:HH:mm}|**{StringHelper.PopulateWithWhiteSpaces(model.NickName.ToUpper(), maxLength)}** через {timeToRespawn.ToString(@"hh\:mm")} | {model.Chance}";
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
