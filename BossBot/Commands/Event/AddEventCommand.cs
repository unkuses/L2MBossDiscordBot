using BossBot.DBModel;
using BossBot.Interfaces;
using BossBot.Model;
using CommonLib.Helpers;
using Newtonsoft.Json;

namespace BossBot.Commands.Event;

public class AddEventCommand(BossData bossData, DateTimeHelper dateTimeHelper) : IEventCommand
{
    const string TimeFormat = "MM/dd:HH:mm";
    public string[] Keys { get; } = ["add", "добавить", "a", "д"];
    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string[] commands)
    {
        if (AddNewEvent(commands, chatId))
            return ["New event added successfully."];
        return ["Failed to add new event. Please check the command format."];
    }

    public async Task<IEnumerable<string>> ExecuteAsync(ulong chatId, ulong userId, string jsonCommand)
    {
        var model = JsonConvert.DeserializeObject<EventModel<AddEventModel>>(jsonCommand);
        var eventInfo = new EventInformationDBModel
        {
            EventName = model.EventCommand.Description,
            Time = model.EventCommand.Time,
            Days = model.EventCommand.RepeatAt,
            ChatId = chatId,
            IsOneTimeEvent = model.EventCommand.RepeatAt == RepeatDays.None,
            TimeBeforeNotification = model.EventCommand.TimeBeforeNotification,
            EventNumber = bossData.LastEventNumber() + 1
        };
        bossData.AddEvent(eventInfo);
        return ["New event added successfully."];
    }

    private bool AddNewEvent(string[] commands, ulong chatId)
    {
        var i = 1;
        var days = ParseRepeatDays(commands, ref i);
        if (!ParseTime(commands[i], out var startTime))
        {
            return false; // Invalid date format
        }

        if(days == RepeatDays.None && startTime < dateTimeHelper.CurrentTime)
        {
            return false; // Event cannot be in the past if no repeat days are specified
        }
        i++;

        if (int.TryParse(commands[i], out var timeBeforeNotification))
        {
            i++;
        }
        else
        {
            timeBeforeNotification = 5; // Default notification time
        }
        if (timeBeforeNotification < 0)
        {
            return false; // Notification time cannot be negative
        }

        var eventName = string.Join(" ", commands.Skip(i));
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return false; // Event name cannot be empty
        }

        var eventInfo = new EventInformationDBModel
        {
            EventName = eventName,
            Time = startTime,
            Days = days,
            ChatId = chatId,
            IsOneTimeEvent = days == RepeatDays.None,
            TimeBeforeNotification = timeBeforeNotification,
            EventNumber = bossData.LastEventNumber() + 1
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

    private bool ParseTime(string str, out DateTime dateTime) =>
        DateTime.TryParseExact(str, TimeFormat, null, System.Globalization.DateTimeStyles.None, out dateTime) || DateTime.TryParse(str, out dateTime);
    
}