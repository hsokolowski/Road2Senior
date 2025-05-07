using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
    
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
        // {
        //     client.BaseAddress = new Uri(configuration.GetSection("Infrastructure")["ApiFootball:BaseUrl"]);
        //     client.DefaultRequestHeaders.Add("X-RapidAPI-Key", configuration["ApiKey"]);
        // });
        //
        // services.AddMemoryCache();
        // services.AddScoped<ICacheService, CacheService>();
        // services.AddScoped<ITimeService, TimeService>();
        // services.AddScoped<ILeagueService, LeagueService>();
        // services.AddScoped<IDatabaseService, DatabaseService.DatabaseService>();
    }
}