using Infrastructure.EndpointClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.EndpointClients;

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
    }
}