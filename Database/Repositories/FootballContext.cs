using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.Repositories
{
    public class FootballContext : DbContext
    {
        public DbSet<LeagueEntity> Leagues { get; set; }

        public FootballContext(DbContextOptions<FootballContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeagueEntity>()
                .HasKey(l => l.Id);

            var databaseProvider = Database.ProviderName;
            if (databaseProvider == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // Dla SQLite: Ustawienie autoincrement
                modelBuilder.Entity<LeagueEntity>()
                    .Property(l => l.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER")
                    .HasAnnotation("Sqlite:Autoincrement", true);
            }
            else if (databaseProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                // Dla SQL Server: Ustawienie Identity
                modelBuilder.Entity<LeagueEntity>()
                    .Property(l => l.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .UseIdentityColumn();
            }

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
    
    
    // var databaseProvider = Database.ProviderName;
    // if (databaseProvider == "Microsoft.EntityFrameworkCore.Sqlite")
    // {
    //     // Dla SQLite: Ustawienie autoincrement
    //     modelBuilder.Entity<LeagueEntity>()
    //         .Property(l => l.Id)
    //         .ValueGeneratedOnAdd()
    //         .HasColumnType("INTEGER")
    //         .HasAnnotation("Sqlite:Autoincrement", true);
    // }
    // else if (databaseProvider == "Microsoft.EntityFrameworkCore.SqlServer")
    // {
    //     // Dla SQL Server: Ustawienie Identity
    //     modelBuilder.Entity<LeagueEntity>()
    //         .Property(l => l.Id)
    //         .ValueGeneratedOnAdd()
    //         .HasColumnType("int")
    //         .UseIdentityColumn();
    // }
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FootballContext>
    {
        public FootballContext CreateDbContext(string[] args)
        {
            // Ustawienie ścieżki bazowej na katalog nadrzędny projektu głównego
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Road2Senior");

            // Wczytanie konfiguracji z głównego katalogu, gdzie znajduje się appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FootballContext>();

            var databaseType = configuration.GetValue<string>("DatabaseType", "Sqlite");

            if (databaseType.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlServer(configuration["SqlServerConnectionString"]);
                return new SqlServerFootballContext(optionsBuilder.Options);
            }
            else
            {
                optionsBuilder.UseSqlite(configuration.GetConnectionString("SqliteConnection"));
                return new SqliteFootballContext(optionsBuilder.Options);
            }
        }
    }
}