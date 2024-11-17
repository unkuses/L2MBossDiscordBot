using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot
{
    public class BossDataSource : DbContext
    {
        private readonly string _dbPath;
        public DbSet<BossDbModel> BossDbModels { get; set; }
        public DbSet<BossInformationDbModel> BossInformationDbModels { get; set; }
        
        public DbSet<UserInformationDBModel> UserInformationDbModels { get; set; }

        public BossDataSource()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Join(path, "L2MBossWithUserTime.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        { 
            options.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BossInformationDbModel>().
                HasMany<BossDbModel>().WithOne();
        }
    }
}
