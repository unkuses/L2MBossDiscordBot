using BossBot.Interfaces;
using Discord.WebSocket;

namespace BossBot.Commands
{
    public class ClearBossCommand(BossData bossData) : ICommand
    {
        public string[] Keys { get; } = ["c", "о"];


        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            bossData.ClearAllBossInformation(channel.Id);
            return Task.CompletedTask;
        }
    }
}
