using Infrastructure.EndpointClients;
using Infrastructure.Football;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Cache;
using Services.EndpointClients;
using Services.Football;
using Services.Timing;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiFootball:BaseUrl"]);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", configuration["ApiFootball:ApiKey"]);
        });
        
        services.AddDbContext<FootballContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IFootballService, FootballService>();
    }
}