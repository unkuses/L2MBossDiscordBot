using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot.DataSource;

public class BossInfoChatsDataSource : DbContext
{
    private readonly string _dbPath;
    public DbSet<BossBotChatsDBModel> BossBotChatsDbModels { get; set; }

    public BossInfoChatsDataSource()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = Path.Join(path, "Chats.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }
}