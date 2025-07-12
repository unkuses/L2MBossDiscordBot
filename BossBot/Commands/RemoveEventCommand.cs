using BossBot.Interfaces;
using BossBot.Model;
using Newtonsoft.Json;

namespace BossBot.Commands;

public class RemoveEventCommand(BossData bossData) : IEventCommand
{
    public string[] Keys { get; } = ["remove", "удалить", "r", "у"];
    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (RemoveEvent(commands))
            return ["Event removed successfully."];
        return ["Failed to remove event. Please check the command format."];
    }

    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string jsonCommand)
    {
        var model = JsonConvert.DeserializeObject<EventModel<RemoveEventModel>>(jsonCommand);
        if(bossData.RemoveEventById(model.EventCommand.EventNumber))
            return ["Event removed successfully."];
        return ["Failed to remove event. Please check the command format."];
    }

    private bool RemoveEvent(string[] commands) => int.TryParse(commands[1], out var result) && bossData.RemoveEventById(result);
}