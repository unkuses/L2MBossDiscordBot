using Discord.WebSocket;

namespace BossBot.Interfaces
{
    public interface ICommand
    {
        string[] Keys { get; }
        Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands);
    }
}
