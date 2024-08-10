using Database.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.EndpointClients;
using WireMock.Server;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public bool UseMockServer { get; set; } = false;
        public WireMockServer MockServer { get; private set; }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                if (UseMockServer)
                {
                    // Start WireMock server
                    MockServer = WireMockServer.Start();

                    // Usunięcie oryginalnego klienta HTTP
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(HttpClient));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Dodanie zamockowanego klienta HTTP, który będzie korzystał z WireMockServer
                    services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
                    {
                        client.BaseAddress = new Uri(MockServer.Url+"/api/football/");
                    });
                }

                // Remove the app's ApplicationDbContext registration.
                var descriptorDbContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FootballContext>));

                if (descriptorDbContext != null)
                {
                    services.Remove(descriptorDbContext);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<FootballContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<FootballContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                }
            });

            return base.CreateHost(builder);
        }

        public new void Dispose()
        {
            if (UseMockServer && MockServer != null)
            {
                MockServer.Stop();
                MockServer.Dispose();
            }

            base.Dispose();
        }
    }
}