using BossBot.Interfaces;
using System.Text;

namespace BossBot.Commands
{
    public class GetAllBossInformationCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["?"];

        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            var list = bossData.GetBossesInformation();
            List<StringBuilder> builders = new List<StringBuilder>();
            StringBuilder builder = new StringBuilder();
            builders.Add(builder);
            foreach (var item in list)
            {
                builder.Append($"**{item.NickName.ToUpper()}**:**{item.Id}**, ");
            }

            return Task.FromResult(builders.Select(b => b.ToString()));
        }
    }
}