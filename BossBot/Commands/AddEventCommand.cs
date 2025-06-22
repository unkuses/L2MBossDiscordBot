using BossBot.DBModel;
using BossBot.Interfaces;

namespace BossBot.Commands;

public class AddEventCommand(BossData bossData) : ICommand
{
    public string[] Keys { get; } = ["add", "добавить", "a", "д"];
    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (AddNewEvent(commands, chatId))
            return ["New event added successfully."];
        return ["Failed to add new event. Please check the command format."];
    }

    private bool AddNewEvent(string[] commands, ulong chatId)
    {
        var i = 1;
        var days = ParseRepeatDays(commands, ref i);
        if (days == RepeatDays.None)
            return false;
        if (!DateTime.TryParse(commands[i], out var startTime))
        {
            return false; // Invalid date format
        }
        var eventName = string.Join(" ", commands.Skip(i + 1));
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return false; // Event name cannot be empty
        }
        var eventInfo = new EventInformationDBModel
        {
            EventName = eventName,
            Time = startTime,
            Days = days,
            ChatId = chatId
        };
        return bossData.AddEvent(eventInfo);
    }



    private RepeatDays ParseRepeatDays(string[] commands, ref int index)
    {
        var days = RepeatDays.None;
        while (index < commands.Length)
        {
            if (Enum.TryParse<RepeatDays>(commands[index], true, out var day))
            {
                days |= day;
            }
            else
            {
                break; // Stop parsing if an invalid day is encountered
            }
            index++;
        }
        return days;
    }
}