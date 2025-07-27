namespace BossBot.Interfaces;

public interface ICommand
{
    string[] Keys { get; }
    Task<List<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands);
}