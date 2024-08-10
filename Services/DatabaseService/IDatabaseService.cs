using Contracts;
using Domain.Entities;
using Domain.Football.Responses;

namespace Services.DatabaseService;

public interface IDatabaseService
{
    Task<LeagueEntity> GetLeagueAsync(int id);
    Task SaveLeaguesToDatabaseAsync(IEnumerable<LeagueModel> leagues);
}