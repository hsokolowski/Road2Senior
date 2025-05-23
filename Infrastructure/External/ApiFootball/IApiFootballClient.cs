using Contracts.ApiFootball;

namespace Infrastructure.External.ApiFootball;

public interface IApiFootballClient
{
    Task<ApiResponse<LeagueResponse>> GetLeagueAsync(int id);
}