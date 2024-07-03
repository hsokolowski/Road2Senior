using System.Text;
using Contracts;
using Domain.Entities;
using Domain.Football.Responses;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace IntegrationTests;

public class FullFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public FullFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
        }).CreateClient();
    }

    [Fact]
    public async Task FullFlow_FromApiClientToGetFromDb()
    {
        // Krok 1: Pobierz ligę z API Football
        var getUrl = "/api/football/leagues?id=1";
        var response = await _client.GetAsync(getUrl);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var leagues = JsonConvert.DeserializeObject<List<LeagueResponse>>(content);

        // Krok 2: Zapisz ligę do bazy danych
        var leagueEntities = leagues.Select(l => new LeagueEntity
        {
            Name = l.League.Name,
            Country = l.Country.Name,
            Season = l.Seasons.FirstOrDefault(s => s.IsCurrent)?.Year ?? l.Seasons.First().Year
        }).ToList();
        var saveUrl = "/api/database/league";
        var saveContent = new StringContent(JsonConvert.SerializeObject(leagueEntities), Encoding.UTF8, "application/json");
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
        Assert.Contains("Premier League", dbContent);
    }
}