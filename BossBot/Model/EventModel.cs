namespace BossBot.Model;

public class EventCommandModel
{
    public EventType Event { get; set; }
}

public class EventModel<T>
{
    public EventType Event { get; set; }
    public T EventCommand { get; set; }
}

public enum EventType
{
    Add,
    Remove,
    All
}