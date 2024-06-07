using Domain.Football.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Football
{
    public interface IFootballService
    {
        Task<IEnumerable<LeagueResponse>> GetLeaguesAsync(int id);
        Task<LeagueEntity> GetLeagueAsync(int id);
        Task SaveLeaguesToDatabaseAsync(IEnumerable<LeagueResponse> leagues);
    }
}