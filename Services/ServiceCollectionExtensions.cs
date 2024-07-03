using Infrastructure.Cache;
using Infrastructure.Timing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.DatabaseService;
using Services.EndpointClients;
using Services.Football;

namespace Services;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiFootball:BaseUrl"]);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", configuration["ApiFootball:ApiKey"]);
        });

        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IFootballService, FootballService>();
        services.AddScoped<IDatabaseService, DatabaseService.DatabaseService>();
    }
}