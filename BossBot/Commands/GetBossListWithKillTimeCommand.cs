using BossBot.Interfaces;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Commands
{
    public class GetBossListWithKillTimeCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["kl"];

        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            var bossListWithKillTime = GetAllBossStatus(bossData.GetAllLoggedBossInfo(channel.Id));
            return channel.SendMessageAsync(bossListWithKillTime);
        }

        private string GetAllBossStatus(IList<BossModel> models)
        {
            StringBuilder statusBuilder = new StringBuilder();
            foreach (var model in models)
            {
                statusBuilder.AppendLine($"!k {model.Id} {model.KillTime:yyyy-MM-dd HH:mm}");
            }

            return statusBuilder.ToString();
        }
    }
}