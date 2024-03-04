using Discord.WebSocket;

namespace BossBot.Interfaces
{
    public interface ICommand
    {
        string[] Keys { get; }
        Task ExecuteAsync(ISocketMessageChannel channel, string[] commands);
    }
}
