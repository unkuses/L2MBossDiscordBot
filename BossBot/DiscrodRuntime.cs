using BossBot.Service;

namespace BossBot;

public class DiscordRuntime(RuntimeService runtimeService, 
    ActivityService activityService, 
    EventChatService eventChatService, 
    BossChatService bossChatService,
    UserStatusAggregatorService userStatusAggregatorService)
{
    public Task MaintenanceTask() => runtimeService.MaintenanceTask();
}