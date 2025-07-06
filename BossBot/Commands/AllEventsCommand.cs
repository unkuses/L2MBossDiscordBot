using BossBot.Interfaces;

namespace BossBot.Commands;

public class GetAllEventsCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = [ "all", "все", "в" ];
    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        var events = bossData.GetAllEvents(chatId);
        return !events.Any() ? ["No events found."] : events.Select(e => $"{e.EventNumber}: {e.EventName} at {e.Time:HH:mm} on {e.Days}");
    }
}