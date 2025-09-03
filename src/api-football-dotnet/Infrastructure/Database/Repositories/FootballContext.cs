using Domain.Entities.League;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class FootballContext : DbContext
{
    public DbSet<LeagueEntity> Leagues { get; set; }

    public FootballContext(DbContextOptions<FootballContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeagueEntity>()
            .HasKey(l => l.Id);

        modelBuilder.Entity<LeagueEntity>()
            .Property(l => l.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();  // Tylko dla SQL Server

        modelBuilder.Entity<LeagueEntity>()
            .Property(l => l.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<LeagueEntity>()
            .Property(l => l.Country)
            .HasMaxLength(50)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}