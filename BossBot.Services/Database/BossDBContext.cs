using BossBot.Services.Model;
using Microsoft.EntityFrameworkCore;

namespace BossBot.Services.Database
{
    public class BossDbContext(DbContextOptions<BossDbContext> options) : DbContext(options)
    {
        public DbSet<Player> Players => Set<Player>();
        public DbSet<EventActivity> EventActivities => Set<EventActivity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Players
            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.ChatId)
                    .HasColumnType("decimal(20,0)")
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode()
                    .IsRequired();
            });

            // EventActivities
            modelBuilder.Entity<EventActivity>(entity =>
            {
                entity.ToTable("EventActivities");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Count);
                entity.Property(e => e.EventName)
                    .IsUnicode().HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany(p => p.EventActivities)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
