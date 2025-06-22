using BossBot.Interfaces;

namespace BossBot.Commands;

public class RemoveEventCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = ["remove", "удалить", "r", "у"];
    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (RemoveEvent(commands))
            return ["Event removed successfully."];
        return ["Failed to remove event. Please check the command format."];
    }

    private bool RemoveEvent(string[] commands) => int.TryParse(commands[1], out var result) && bossData.RemoveEventById(result);
}