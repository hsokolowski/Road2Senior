using Contracts.ApiFootball;

namespace Application.External;

public interface IApiFootballClient
{
    Task<ApiResponse<LeagueResponse>> GetLeagueAsync(int id);
}