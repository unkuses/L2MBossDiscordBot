namespace BossBot.Interfaces;

internal interface IEventCommand
{
    string[] Keys { get; }
    Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands);

    Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string jsonCommand);
}