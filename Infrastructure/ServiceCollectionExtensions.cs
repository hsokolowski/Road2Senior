using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
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
}