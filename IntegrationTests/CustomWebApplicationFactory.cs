using Database.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        private string _apiKey;

        public CustomWebApplicationFactory()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") 
                .Build();

            _apiKey = config.GetSection("Infrastructure:ApiFootball").GetValue<string>("ApiKey");
        }
        
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
                
                services.AddHttpClient("TestClient", client =>
                {
                    client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
                });
            });

            return base.CreateHost(builder);
        }
        
        public HttpClient CreateClientWithApiKey()
        {
            var client = this.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = false,
                BaseAddress = new Uri("http://localhost")
            });

            client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

            return client;
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