using BossBot.Interfaces;

namespace BossBot.Commands
{
    public class ClearBossCommand(CosmoDb bossData) : ICommand
    {
        public string[] Keys { get; } = ["c", "о"];


        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
        {
            _ = bossData.ClearAllBossInformationAsync(chatId);
            List<string> answer = ["Все тайминги были сброшены"];
            return Task.FromResult(answer.Select(s => s));
        }
    }
}
