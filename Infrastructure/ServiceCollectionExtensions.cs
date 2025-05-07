using Application.Database;
using Application.External;
using Contracts.Cache;
using Contracts.Timing;
using Infrastructure.Cache;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;
using Infrastructure.External.ApiFootball;
using Infrastructure.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<ITimeService, SystemTimeService>();
        services.AddScoped<ILeagueRepository, LeagueRepository>();

        services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiFootball:BaseUrl"]);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", configuration["ApiKey"]);
        });

        return services;
    }
    
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        if (string.Equals(environmentName, "Test", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<FootballContext>(options =>
                options.UseInMemoryDatabase("InMemoryDbForTesting"));
            return services;
        }

        var dbType = configuration["DatabaseType"];

        switch (dbType)
        {
            case "AzureSql":
                var azureConnection = configuration.GetConnectionString("AzureSql");
                services.AddDbContext<FootballContext>(options =>
                    options.UseSqlServer(azureConnection));
                break;

            case "SqlServer":
                var localConnection = configuration.GetConnectionString("SqlServerConnection");
                services.AddDbContext<FootballContext>(options =>
                    options.UseSqlServer(localConnection));
                break;

            default:
                throw new InvalidOperationException($"Unsupported DatabaseType: {dbType}");
        }

        return services;
    }
    // public static void RegisterDb(this IServiceCollection services, IConfiguration configuration, string environmentName)
    // {
    //     // Środowisko testowe = InMemory DB
    //     if (string.Equals(environmentName, "Test", StringComparison.OrdinalIgnoreCase))
    //     {
    //         services.AddDbContext<FootballContext>(options =>
    //             options.UseInMemoryDatabase("InMemoryDbForTesting"));
    //         return;
    //     }
    //
    //     var dbType = configuration["DatabaseType"];
    //
    //     switch (dbType)
    //     {
    //         case "AzureSql":
    //             var azureConnection = configuration.GetConnectionString("AzureSql");
    //             services.AddDbContext<FootballContext>(options =>
    //                 options.UseSqlServer(azureConnection));
    //             break;
    //
    //         case "SqlServer":
    //             var localConnection = configuration.GetConnectionString("SqlServerConnection");
    //             services.AddDbContext<FootballContext>(options =>
    //                 options.UseSqlServer(localConnection));
    //             break;
    //
    //         default:
    //             throw new InvalidOperationException($"Unsupported DatabaseType: {dbType}");
    //     }
    // }
    
    public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
        // {
        //     client.BaseAddress = new Uri(configuration["ApiFootball:BaseUrl"]);
        //     client.DefaultRequestHeaders.Add("X-RapidAPI-Key", configuration["ApiFootball:ApiKey"]);
        // });
        //
        // services.AddDbContext<FootballContext>(options =>
        //     options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        // services.AddMemoryCache();
        // services.AddScoped<ICacheService, CacheService>();
        // services.AddScoped<ITimeService, TimeService>();
        // services.AddScoped<IFootballService, FootballService>();
        // services.AddScoped<IDatabaseService, DatabaseService>();
    }
    
    // public static void RegisterDb(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    // {
        // if (environment.IsEnvironment("Test"))
        // {
        //     // Testy integracyjne używają InMemoryDb
        //     services.AddDbContext<FootballContext>(options =>
        //         options.UseInMemoryDatabase("InMemoryDbForTesting"));
        //     return;
        // }
        //
        // //  Pobranie connection stringa do SQL Server
        // var sqlServerConnectionString = configuration.GetConnectionString("SqlServerConnection");
        //
        // if (string.IsNullOrEmpty(sqlServerConnectionString))
        // {
        //     throw new InvalidOperationException("No connection string in Key Vault!");
        // }
        //
        // // Produkcja i lokalne środowisko używają SQL Server
        // services.AddDbContext<FootballContext>(options =>
        //     options.UseSqlServer(sqlServerConnectionString));
    //}
}