using BossBot.Interfaces;
using System.Text;

namespace BossBot.Commands
{
    public class GetAllBossInformationCommand(CosmoDb bossData) : ICommand
    {
        public string[] Keys { get; } = ["?"];

        public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
        {
            var list = await bossData.GetBossesInformationAsync();
            List<StringBuilder> builders = new List<StringBuilder>();
            StringBuilder builder = new StringBuilder();
            builders.Add(builder);
            foreach (var item in list)
            {
                builder.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }

            return builders.Select(b => b.ToString());
        }
    }
}