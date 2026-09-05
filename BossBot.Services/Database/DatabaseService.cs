using BossBot.Services.Model;
using CommonLib.Models;
using Microsoft.EntityFrameworkCore;

namespace BossBot.Services.Database;

public class DatabaseService(IDbContextFactory<BossDbContext> factory)
{
    public async Task<List<string>> GetAllPlayersNamesAsync(ulong chatId)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Players
            .Where(p => p.ChatId == chatId)
            .Select(p => p.Name)
            .ToListAsync();
    }

    public async Task AddPlayerActivitiesAsync(ulong chatId, List<string> playersNames, string eventName)
    {
        await using var context = await factory.CreateDbContextAsync();
        var players = await context.Players
            .Where(p => p.ChatId == chatId && playersNames.Contains(p.Name))
            .ToListAsync();
        foreach (var player in players)
        {
            var eventActivity = await context.EventActivities
                .FirstOrDefaultAsync(e => e.UserId == player.Id && e.EventName == eventName);
            if (eventActivity == null)
            {
                await context.EventActivities.AddAsync(new EventActivity
                {
                    Id = Guid.NewGuid(),
                    UserId = player.Id,
                    EventName = eventName,
                    Count = 1
                });
            }
            else
            {
                eventActivity.Count += 1;
                context.EventActivities.Update(eventActivity);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<string>> AddNewPlayerNameAsync(ulong chatId, List<string> playerNames)
    {
        var result = new List<string>();
        await using var context = await factory.CreateDbContextAsync();
        foreach (var playerName in playerNames)
        {
            var playerNameSmall = playerName.Replace(" ", "").ToLower();
            var existingPlayer =
                await context.Players.FirstOrDefaultAsync(p => p.ChatId == chatId && p.Name == playerNameSmall);
            if (existingPlayer != null)
            {
                continue;
            }

            await context.Players.AddAsync(new Player
            {
                ChatId = chatId,
                Name = playerNameSmall
            });
            result.Add(playerNameSmall);
        }

        await context.SaveChangesAsync();
        return result;
    }

    public async Task CleanUsersByChatId(ulong chatId)
    {
        await using var context = await factory.CreateDbContextAsync();
        var users = await context.Players.Where(p => p.ChatId == chatId).ToListAsync();
        var userIds = users.Select(u => u.Id).ToList();
        var eventActivities = context.EventActivities.Where(e => userIds.Contains(e.UserId));
        context.EventActivities.RemoveRange(eventActivities);
        context.Players.RemoveRange(users);
        await context.SaveChangesAsync();
    }

    public async Task<List<EventStatistic>> GetUserStatisticByEventName(ulong chatId, string eventName)
    {
        await using var context = await factory.CreateDbContextAsync();
        var result = await context.EventActivities
            .Include(p => p.User)
            .Where(e => e.EventName == eventName && e.User.ChatId == chatId)
            .ToListAsync();

        return result.Select(e => new EventStatistic
        {
            PlayerName = e.User.Name,
            Count = e.Count
        }).ToList();
    }

    public async Task<UserStatistic?> GetStatisticByUserNameAsync(ulong chatId, string userName)
    {
        var normalizedName = userName.Replace(" ", string.Empty).ToLowerInvariant();
        await using var context = await factory.CreateDbContextAsync();
        var player = await context.Players
            .AsNoTracking()
            .Include(p => p.EventActivities)
            .FirstOrDefaultAsync(p => p.ChatId == chatId && p.Name == normalizedName);

        if (player == null)
            return null;

        var events = player.EventActivities
            .GroupBy(activity => activity.EventName)
            .Select(group => new EventActivityStatistic
            {
                EventName = group.Key,
                Count = group.Sum(activity => activity.Count)
            })
            .OrderByDescending(activity => activity.Count)
            .ThenBy(activity => activity.EventName)
            .ToList();

        return new UserStatistic
        {
            PlayerName = player.Name,
            TotalCount = events.Sum(activity => activity.Count),
            Events = events
        };
    }
}
