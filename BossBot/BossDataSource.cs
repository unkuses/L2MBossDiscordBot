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
        
        public DbSet<BossNamesDBModel> BossNamesDbModels { get; set; }

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
            modelBuilder.Entity<BossInformationDbModel>()
                .HasOne(a => a.Boss)               
                .WithMany(p => p.BossInformationDbModels)          
                .HasForeignKey(a => a.BossId)      
                .OnDelete(DeleteBehavior.Cascade);   
            
            modelBuilder.Entity<BossNamesDBModel>()
                .HasOne(b => b.Boss)              
                .WithMany(p => p.BossNames)
                .HasForeignKey(b => b.BossId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
