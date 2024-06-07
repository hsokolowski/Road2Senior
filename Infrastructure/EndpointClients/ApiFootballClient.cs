using Domain.Football.Responses;
using Newtonsoft.Json;
using Services.EndpointClients;

namespace Infrastructure.EndpointClients;

public class ApiFootballClient : IApiFootballClient
{
    private readonly HttpClient _httpClient;

    public ApiFootballClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<LeagueResponse>> GetLeagueAsync(int id)
    {
        var url = new Uri(_httpClient.BaseAddress, $"leagues?id={id}");
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-RapidApi-Host", "api-football-vq.p.rapidapi.com" );
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<ApiResponse<LeagueResponse>>(content) ??
               throw new InvalidOperationException();
    }
}