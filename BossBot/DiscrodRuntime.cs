using BossBot.Service;

namespace BossBot;

public class DiscordRuntime(RuntimeService runtimeService,
    NewActivityService activityService, 
    EventChatService eventChatService, 
    BossChatService bossChatService,
    UserStatusAggregatorService userStatusAggregatorService)
{
    // These services subscribe to Discord events when constructed. Keep references so their
    // lifetime is explicitly tied to the runtime singleton.
    private readonly NewActivityService _activityService = activityService;
    private readonly EventChatService _eventChatService = eventChatService;
    private readonly BossChatService _bossChatService = bossChatService;
    private readonly UserStatusAggregatorService _userStatusAggregatorService = userStatusAggregatorService;

    public List<Task> MaintenanceTask()
    {
        return
        [
            runtimeService.StartDailyJob(),
            runtimeService.MaintenanceTask()
        ];
    }
}
