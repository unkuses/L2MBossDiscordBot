using BossBot.Interfaces;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Commands
{
    public class GetBossListWithKillTimeCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["kl"];

        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands) =>
            GetAllBossStatus(bossData.GetAllLoggedBossInfo(chatId));

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