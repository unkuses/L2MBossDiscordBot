using BossBot.Interfaces;
using Discord.WebSocket;
using System.Text;

namespace BossBot.Commands
{
    public class GetAllNotLoggedBossesCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["??"];
        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            var list = bossData.GetAllNotLoggedBossInformation(chatId);
            var stringBuilders = new List<StringBuilder>();
            var str = new StringBuilder();
            stringBuilders.Add(str);
            foreach (var item in list)
            {
                str.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }

            return Task.FromResult(stringBuilders.Select(s => s.ToString()));
        }
    }
}
