using Database.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Database;

public static class ServiceCollectionExtensions
{
    public static void RegisterDb(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Pobierz typ bazy danych z konfiguracji
        var databaseType = configuration.GetValue<string>("DatabaseType", "Sqlite");

        if (environment.IsEnvironment("Test"))
        {
            // Użycie InMemory w testach
            services.AddDbContext<FootballContext>(options =>
                options.UseInMemoryDatabase("InMemoryDbForTesting"));
        }
        else if (databaseType.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            // Użycie SQL Server w produkcji
            services.AddDbContext<FootballContext>(options =>
                options.UseSqlServer(configuration["SqlServerConnectionString"],
                    b => b.MigrationsAssembly("Database.Migrations.SqlServerMigrations")));
        }
        else if (databaseType.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            // Użycie SQLite w lokalnym środowisku deweloperskim
            services.AddDbContext<FootballContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("SqliteConnection"),
                    b => b.MigrationsAssembly("Database.Migrations.SqliteMigrations")));
        }
    }
}