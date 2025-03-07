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
        if (environment.IsEnvironment("Test"))
        {
            // Testy integracyjne używają InMemoryDb
            services.AddDbContext<FootballContext>(options =>
                options.UseInMemoryDatabase("InMemoryDbForTesting"));
            return;
        }

        //  Pobranie connection stringa do SQL Server
        var sqlServerConnectionString = configuration.GetConnectionString("SqlServerConnection");

        if (string.IsNullOrEmpty(sqlServerConnectionString))
        {
            throw new InvalidOperationException("No connection string in Key Vault!");
        }

        // Produkcja i lokalne środowisko używają SQL Server
        services.AddDbContext<FootballContext>(options =>
            options.UseSqlServer(sqlServerConnectionString));
    }
}