using Domain.Football.Responses;

namespace Services.EndpointClients;

public interface IApiFootballClient
{
    Task<ApiResponse<LeagueResponse>> GetLeagueAsync(int id);
}