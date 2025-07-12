using BossBot.Interfaces;

namespace BossBot.Commands;

public class GetAllEventsCommand(BossData bossData) : IEventCommand
{
    public string[] Keys { get; } = [ "all", "все", "в" ];
    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands) =>
        GetAllEvents(chatId);

    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string jsonCommand) =>
        GetAllEvents(chatId);

    private IEnumerable<string> GetAllEvents(ulong chatId)
    {
        var events = bossData.GetAllEvents(chatId);
        return !events.Any() ? ["No events found."] : events.Select(e => $"{e.EventNumber}: {e.EventName} at {e.Time:HH:mm} on {e.Days}");
    }
}