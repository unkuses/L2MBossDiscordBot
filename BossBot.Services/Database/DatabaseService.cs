using BossBot.Services.Model;
using CommonLib.Models;
using Microsoft.EntityFrameworkCore;

namespace BossBot.Services.Database;

public class DatabaseService(IDbContextFactory<BossDbContext> factory)
{
    public async Task<List<string>> GetAllPlayersNamesAsync(ulong chatId)
    {
        var context = await factory.CreateDbContextAsync();
        return await context.Players
            .Where(p => p.ChatId == chatId)
            .Select(p => p.Name)
            .ToListAsync();
    }

    public async Task AddPlayerActivitiesAsync(ulong chatId, List<string> playersNames, string eventName)
    {
        var context = await factory.CreateDbContextAsync();
        var players = context.Players.Where(p => p.ChatId == chatId && playersNames.Contains(p.Name)).ToList();
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
        var context = await factory.CreateDbContextAsync();
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
            await context.SaveChangesAsync();
        }

        return result;
    }

    public async Task CleanUsersByChatId(ulong chatId)
    {
        var context = await factory.CreateDbContextAsync();
        var users = context.Players.Where(p => p.ChatId == chatId);
        var userIds = users.Select(u => u.Id).ToList();
        var eventActivities = context.EventActivities.Where(e => userIds.Contains(e.UserId));
        context.EventActivities.RemoveRange(eventActivities);
        await context.SaveChangesAsync();
        context.Players.RemoveRange(users);
        await context.SaveChangesAsync();
    }

    public async Task<List<EventStatistic>> GetUserStatisticByEventName(ulong chatId, string eventName)
    {
        var context = await factory.CreateDbContextAsync();
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
}