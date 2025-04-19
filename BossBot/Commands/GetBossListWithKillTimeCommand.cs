using BossBot.Interfaces;
using System.Text;
using CommonLib.Models;

namespace BossBot.Commands
{
    public class GetBossListWithKillTimeCommand(CosmoDb bossData) : ICommand
    {
        public string[] Keys { get; } = ["kl"];

        public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands) =>
            await GetAllBossStatus(await bossData.GetAllLoggedBossInfoAsync(chatId));

        private Task<IEnumerable<string>> GetAllBossStatus(IList<BossModel> models)
        {
            var stringBuilders = new List<StringBuilder>();
            StringBuilder statusBuilder = new StringBuilder();
            stringBuilders.Add(statusBuilder);
            foreach (var model in models)
            {
                var str = $"!k {model.Id} {model.KillTime:yyyy-MM-dd HH:mm}";
                if (statusBuilder.Length + str.Length > 2000)
                {
                    statusBuilder = new StringBuilder();
                }

                statusBuilder.AppendLine(str);
            }

            return Task.FromResult(stringBuilders.Select(s => s.ToString()));
        }
    }
}