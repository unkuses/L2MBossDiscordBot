using BossBot.DataSource;
using BossBot.DBModel;

namespace BossBot;

public class UserStatisticData
{
    private readonly UserStatisticDataSource _userStatisticDataSource = new();
    public UserStatisticData()
    {
        _userStatisticDataSource.Database.EnsureCreated();
    }

    public void AddUserStatistic(ulong chatId, string userName)
    {
        var user = FindUserByName(chatId, userName);
        if (user != null)
        {
            user.Count++;
            _userStatisticDataSource.UserInfo.Update(user);
            _userStatisticDataSource.SaveChanges();
            return;
        }

        var count = _userStatisticDataSource.UserInfo.Count() + 1;
        _userStatisticDataSource.UserInfo.Add(new UserStatisticDBModel() { ChatId = chatId, Count = 1, UserName = userName, UserId = count });
        _userStatisticDataSource.SaveChanges();
    }

    public bool ClearAllInformation(ulong chatId)
    {
        _userStatisticDataSource.UserInfo.RemoveRange(_userStatisticDataSource.UserInfo.Where(u => u.ChatId == chatId));
        return _userStatisticDataSource.SaveChanges() > 0;
    }

    public bool RemoveUser(ulong chatId, int userId)
    {
        var user = _userStatisticDataSource.UserInfo.FirstOrDefault(u => u.UserId == userId && u.ChatId == chatId);
        if (user != null)
        {
            _userStatisticDataSource.UserInfo.Remove(user);
            _userStatisticDataSource.SaveChanges();
            RebuildUserIds();
            return true;
        }
        return false;
    }
    public List<UserStatisticDBModel> GetUserStatistics(ulong chatId) =>
        _userStatisticDataSource.UserInfo.Where(u => u.ChatId == chatId).OrderByDescending(u => u.Count).ToList();

    public bool CreateOrUpdateUser(ulong chatId, string userName, int count)
    {
        var user = FindUserByName(chatId, userName);
        if (user != null)
        {
            user.Count = count;
            _userStatisticDataSource.UserInfo.Update(user);
        }
        else
        {
            var newUserId = _userStatisticDataSource.UserInfo.Count() + 1;
            _userStatisticDataSource.UserInfo.Add(new UserStatisticDBModel() { ChatId = chatId, Count = count, UserName = userName, UserId = newUserId });
        }
        return _userStatisticDataSource.SaveChanges() > 0;
    }

    public bool MergeUsers(ulong chatId, int firstUserId, int secondUserId)
    {
        var firstUser = _userStatisticDataSource.UserInfo.FirstOrDefault(u => u.UserId == firstUserId && u.ChatId == chatId);
        var secondUser = _userStatisticDataSource.UserInfo.FirstOrDefault(u => u.UserId == secondUserId && u.ChatId == chatId);
        if (firstUser == null || secondUser == null)
            return false;
        firstUser.Count += secondUser.Count;
        _userStatisticDataSource.UserInfo.Remove(secondUser);
        _userStatisticDataSource.UserInfo.Update(firstUser);
        _userStatisticDataSource.SaveChanges();
        RebuildUserIds();
        return true;
    }

    private void RebuildUserIds()
    {
        var newId = 1;
        _userStatisticDataSource.UserInfo.ToList().ForEach(u =>
        {
            u.UserId = newId++;
            _userStatisticDataSource.UserInfo.Update(u);
        });
        _userStatisticDataSource.SaveChanges();
    }

    private UserStatisticDBModel? FindUserByName(ulong chatId, string userName)
    {
        var users = _userStatisticDataSource.UserInfo.Where(u => u.ChatId == chatId);
        foreach (var user in users)
        {
            if(user.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                return user;
        }

        return null;
    }
}