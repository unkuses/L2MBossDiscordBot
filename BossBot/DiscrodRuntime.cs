using BossBot.Service;

namespace BossBot;

public class DiscordRuntime(RuntimeService runtimeService, ActivityService activityService, EventChatService eventChatService, BossChatService bossChatService)
{
    public Task MaintenanceTask() => runtimeService.MaintenanceTask();
}