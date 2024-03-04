using BossBot.Interfaces;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Commands
{
    public class GetAllBossInformationCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["?"];

        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            var list = bossData.GetBossesInformation();
            StringBuilder str = new StringBuilder();
            foreach (var item in list)
            {
                str.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }

            return channel.SendMessageAsync(str.ToString());
        }
    }
}