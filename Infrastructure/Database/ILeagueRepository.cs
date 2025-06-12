using Contracts.League;
using Domain.Entities.League;

namespace Infrastructure.Database;

public interface ILeagueRepository
{
    Task<LeagueEntity> GetLeagueAsync(int id);
    Task SaveLeaguesToDatabaseAsync(IEnumerable<LeagueDto> leagues);
}