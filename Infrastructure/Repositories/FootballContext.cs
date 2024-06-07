using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Repositories
{
    public class FootballContext : DbContext
    {
        public DbSet<LeagueEntity> Leagues { get; set; }

        public FootballContext(DbContextOptions<FootballContext> options) : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=FootballDB.db", b => b.MigrationsAssembly("Infrastructure"));
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeagueEntity>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<LeagueEntity>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }
    }
    
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FootballContext>
    {
        public FootballContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FootballContext>();
            optionsBuilder.UseSqlite("Data Source=FootballDB.db", b => b.MigrationsAssembly("Infrastructure"));

            return new FootballContext(optionsBuilder.Options);
        }
    }
}