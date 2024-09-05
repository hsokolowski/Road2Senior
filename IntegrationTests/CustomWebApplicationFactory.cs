using Azure.Identity;
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
        // Flaga do używania MockServer
        public bool UseMockServer { get; set; } = false;

        // MockServer dla testów
        public WireMockServer MockServer { get; private set; }

        // Przechowywanie klucza API
        private string _apiKey;

        // Konstruktor fabryki, który wczytuje klucz API z appsettings.json
        public CustomWebApplicationFactory()
        {
            // Konfiguracja configu z pliku appsettings.json i KV
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Wczytaj plik appsettings.json
                .AddAzureKeyVault(new Uri("https://apifootball.vault.azure.net/"), new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeVisualStudioCredential = true,  // Wyłącz Visual Studio Credential, którego nie masz w pipeline
                    ExcludeEnvironmentCredential = false,  // Włącz EnvironmentCredential, który bazuje na zmiennych środowiskowych
                    ExcludeManagedIdentityCredential = true  // Wyłącz Managed Identity Credential, jeśli nie korzystasz z Managed Identity
                })) // Wczytaj KV 
                .Build();

            // Pobierz wartość klucza API z konfiguracji
            _apiKey = config.GetValue<string>("ApiKey");

            // Sprawdzenie, czy klucz API został poprawnie wczytany
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("ApiKey nie został poprawnie wczytany z pliku appsettings.json.");
            }
        }

        // Metoda tworząca hosta testowego
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                if (UseMockServer)
                {
                    // Rozpocznij działanie MockServer, jeśli jest w użyciu
                    MockServer = WireMockServer.Start();

                    // Usuń oryginalnego klienta HTTP
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(HttpClient));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Dodaj zamockowanego klienta HTTP, który używa MockServer
                    services.AddHttpClient<IApiFootballClient, ApiFootballClient>(client =>
                    {
                        client.BaseAddress = new Uri(MockServer.Url + "/api/football/");
                    });
                }

                // Usuń kontekst bazy danych aplikacji, jeśli istnieje
                var descriptorDbContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<FootballContext>));

                if (descriptorDbContext != null)
                {
                    services.Remove(descriptorDbContext);
                }

                // Dodaj kontekst bazy danych in-memory do testów
                services.AddDbContext<FootballContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Zbuduj usługodawcę
                var serviceProvider = services.BuildServiceProvider();

                // Stwórz zakres i upewnij się, że baza danych jest utworzona
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<FootballContext>();

                    // Upewnij się, że baza danych została utworzona
                    db.Database.EnsureCreated();
                }
                
                // Dodanie klienta HTTP z nagłówkiem X-Api-Key
                services.AddHttpClient("TestClient", client =>
                {
                    client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
                });
            });

            return base.CreateHost(builder);
        }

        // Metoda do tworzenia klienta HTTP z kluczem API
        public HttpClient CreateClientWithApiKey()
        {
            var client = this.WithWebHostBuilder(builder =>
            {
                // Ustawienie środowiska na testowe
                builder.UseEnvironment("Test");
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = false,
                BaseAddress = new Uri("http://localhost")
            });

            // Dodanie nagłówka X-Api-Key do żądań
            client.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

            return client;
        }

        // Zatrzymanie MockServer i czyszczenie zasobów po zakończeniu testów
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