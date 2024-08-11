using System.Text;
using System.Text.Json;
using Contracts;
using Domain.Entities;
using Domain.Football.Responses;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationTests;

public class FullFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public FullFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClientWithApiKey();
    }

    [Fact]
    public async Task FullFlow_FromApiClientToGetFromDb()
    {
        // Krok 1: Pobierz ligę z API Football
        var getUrl = "/api/football/leagues?id=1";
        var response = await _client.GetAsync(getUrl);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var leagues =  JsonSerializer.Deserialize<IEnumerable<LeagueModel>>(content, new JsonSerializerOptions {AllowTrailingCommas = true, PropertyNameCaseInsensitive = true});

        // Krok 2: Zapisz ligę do bazy danych
        var saveUrl = "/api/database/league";
        var saveContent = new StringContent( JsonSerializer.Serialize(leagues), Encoding.UTF8, "application/json");
        var saveResponse = await _client.PostAsync(saveUrl, saveContent);
        
        if (!saveResponse.IsSuccessStatusCode)
        {
            var errorContent = await saveResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Error response: " + errorContent);
        }
        
        saveResponse.EnsureSuccessStatusCode();

        // Krok 3: Odczytaj ligę z bazy danych
        var getFromDbUrl = "/api/database/league/1";
        var getFromDbResponse = await _client.GetAsync(getFromDbUrl);
        getFromDbResponse.EnsureSuccessStatusCode();
        var dbContent = await getFromDbResponse.Content.ReadAsStringAsync();
        Assert.Contains("World Cup", dbContent);
    }
}