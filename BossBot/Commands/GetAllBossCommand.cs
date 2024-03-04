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
        
        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            if (commands.Length == 1 || !int.TryParse(commands[1], out var count))
            {
                return GetBossInformation(channel);
            }
            else
            {
                return GetFirstLoggedBossInfo(channel, count);
            }
        }

        private Task GetBossInformation(ISocketMessageChannel channel)
        {
            var bosses = bossData.GetAllLoggedBossInfo(channel.Id);
            var messages = PopulateBossInformationString(bosses);
            messages.ForEach(m => { channel.SendMessageAsync(m.ToString()); });
            return Task.CompletedTask;
        }

        private Task GetFirstLoggedBossInfo(ISocketMessageChannel channel, int count)
        {
            if (count <= 0)
            {
                return Task.CompletedTask;
            }

            var bosses = bossData.GetFirstLoggedBossInfo(channel.Id, count);
            var messages = PopulateBossInformationString(bosses);
            messages.ForEach(m => { channel.SendMessageAsync(m.ToString()); });
            return Task.CompletedTask;
        }

        private List<StringBuilder> PopulateBossInformationString(IList<BossModel> models)
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

            return builders;
        }
    }
}
