using BossBot.Interfaces;
using System.Text;
using CommonLib.Models;

namespace BossBot.Commands.BossInfo
{
    public class GetBossListWithKillTimeCommand(CosmoDb bossData) : ICommand
    {
        public string[] Keys { get; } = ["kl"];

        public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands) =>
            await GetAllBossStatus(await bossData.GetAllLoggedBossInfoAsync(chatId));

        private Task<List<string>> GetAllBossStatus(IList<BossModel> models)
        {
            var stringBuilders = new List<StringBuilder>();
            var statusBuilder = new StringBuilder();
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

            return Task.FromResult(stringBuilders.Select(s => s.ToString()).ToList());
        }
    }
}