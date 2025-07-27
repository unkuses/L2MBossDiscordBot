using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot.DataSource;

internal class UserStatisticDataSource : DbContext
{
    private readonly string _dbPath;
    public DbSet<UserStatisticDBModel> UserInfo { get; set; }

    public UserStatisticDataSource()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = Path.Join(path, "UserStatistic.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }
}
