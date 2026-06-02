using BossBot.Service;

namespace BossBot;

public class DiscordRuntime(RuntimeService runtimeService,
    NewActivityService activityService, 
    EventChatService eventChatService, 
    BossChatService bossChatService,
    UserStatusAggregatorService userStatusAggregatorService)
{
    public List<Task> MaintenanceTask()
    {
        return
        [
            runtimeService.StartDailyJob(),
            runtimeService.MaintenanceTask()
        ];
    }
}