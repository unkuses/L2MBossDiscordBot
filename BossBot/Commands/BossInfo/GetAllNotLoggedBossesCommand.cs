using BossBot.Interfaces;
using System.Text;

namespace BossBot.Commands.BossInfo
{
    public class GetAllNotLoggedBossesCommand(CosmoDb bossData) : ICommand
    {
        public string[] Keys { get; } = ["??"];
        public async Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
        {
            var list = await bossData.GetAllNotLoggedBossInformationAsync(chatId);
            var stringBuilders = new List<StringBuilder>();
            var str = new StringBuilder();
            stringBuilders.Add(str);
            foreach (var item in list)
            {
                str.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }

            return stringBuilders.Select(s => s.ToString()).ToList();
        }
    }
}
