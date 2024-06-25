using Domain.Football.Responses;

namespace Infrastructure.EndpointClients;

public interface IApiFootballClient
{
    Task<ApiResponse<LeagueResponse>> GetLeagueAsync(int id);
}