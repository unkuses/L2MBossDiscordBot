using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot.DataSource
{
    public class LanguageDataSource : DbContext
    {
        private readonly string _dbPath;
        public DbSet<ChatLanguageDBModel> ChatLanguageInfo { get; set; }

        public LanguageDataSource()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Join(path, "ChatLanguage.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_dbPath}");
        }
    }
}
