using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot.DataSource
{
    public class BossDataSource : DbContext
    {
        private readonly string _dbPath;
        public DbSet<UserInformationDBModel> UserInformationDbModels { get; set; }
        
        public BossDataSource()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Join(path, "UserInformation.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        { 
            options.UseSqlite($"Data Source={_dbPath}");
        }
    }
}
