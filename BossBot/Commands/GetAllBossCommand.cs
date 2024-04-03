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
            return BossUtils.PopulateBossInformationString(bosses, dateTimeHelper);
        }

        private Task<IEnumerable<string>> GetFirstLoggedBossInfo(ulong chatId, int count)
        {
            if (count <= 0)
            {
                return Task.FromResult(Enumerable.Empty<string>());
            }

            var bosses = bossData.GetFirstLoggedBossInfo(chatId, count);
            return BossUtils.PopulateBossInformationString(bosses, dateTimeHelper);
        }


    }
}
