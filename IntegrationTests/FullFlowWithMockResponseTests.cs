﻿using System.Text.Json;
using Contracts;
using Domain.Football.Responses;
using Microsoft.AspNetCore.Hosting;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace IntegrationTests;

public class FullFlowWithMockResponseTests: IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public FullFlowWithMockResponseTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _factory.UseMockServer = true;
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
        }).CreateClient();

        SetupMockResponse();
    }

    private void SetupMockResponse()
    {
        var mockResponse = new ApiResponse<LeagueResponse>
        {
            Get = "leagues",
            Parameters = new Dictionary<string, string> { { "id", "1" } },
            Results = 1,
            Response = new List<LeagueResponse>
            {
                new LeagueResponse
                {
                    League = new League { Id = 1, Name = "Premiere League", Type = "League" },
                    Country = new Country { Name = "England", Code = null },
                    Seasons = new List<Season>
                    {
                        new Season { Year = 2010, StartDate = "2010-06-11", EndDate = "2010-07-11", IsCurrent = false },
                        new Season { Year = 2014, StartDate = "2014-06-12", EndDate = "2014-07-13", IsCurrent = false },
                        new Season { Year = 2018, StartDate = "2018-06-14", EndDate = "2018-07-15", IsCurrent = false },
                        new Season { Year = 2022, StartDate = "2022-11-20", EndDate = "2022-12-18", IsCurrent = true }
                    }
                }
            }
        };

        _factory.MockServer
            .Given(Request.Create().WithPath("/api/football/leagues").UsingGet()
                .WithParam("id", "1")) 
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(JsonSerializer.Serialize(mockResponse)));
    }


    [Fact]
    public async Task MockedApi_ShouldReturnExpectedResponse()
    {
        // Użycie zamockowanego API
        var getUrl = "/api/football/leagues?id=1";
        var response = await _client.GetAsync(getUrl);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Deserializacja odpowiedzi
        var leagues =  JsonSerializer.Deserialize<IEnumerable<LeagueModel>>(content, new JsonSerializerOptions {AllowTrailingCommas = true, PropertyNameCaseInsensitive = true});
        
        // Sprawdzenie, czy odpowiedź jest zgodna z oczekiwaniami
        Assert.NotNull(leagues);
        Assert.Single(leagues);
        Assert.Equal("Premiere League", leagues.First().Name);
        Assert.Equal("England", leagues.First().Country);
    }
    
    public void Dispose()
    {
        _factory.Dispose();
    }
}