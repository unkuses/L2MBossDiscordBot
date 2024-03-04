using BossBot.Interfaces;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Commands
{
    public class GetAllNotLoggedBossesCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["??"];
        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            var list = bossData.GetAllNotLoggedBossInformation(channel.Id);
            var str = new StringBuilder();
            foreach (var item in list)
            {
                str.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }

            channel.SendMessageAsync(str.ToString());
            return Task.CompletedTask;
        }
    }
}
