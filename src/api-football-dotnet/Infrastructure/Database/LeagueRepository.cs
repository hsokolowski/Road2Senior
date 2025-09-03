using Contracts;
using Contracts.League;
using Domain.Entities.League;
using Infrastructure.Database.Repositories;
using Infrastructure.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

public class LeagueRepository : ILeagueRepository
{
    private FootballContext _context;
    private readonly ITimeService _timeService;
    private readonly ILogger<LeagueRepository> _logger;

    public LeagueRepository(FootballContext context, ITimeService timeService, ILogger<LeagueRepository> logger)
    {
        _context = context;
        _timeService = timeService;
        _logger = logger;
    }
    public async Task<LeagueEntity> GetLeagueAsync(int id)
    {
        return await _context.Leagues.FindAsync(id);
    }
    
    public async Task SaveLeaguesToDatabaseAsync(IEnumerable<LeagueDto> leagues)
    {
        var leagueEntities = leagues.Select(l => new LeagueEntity
        {
            Name = l.Name,
            Country = l.Country,
            Season = l.Season
        }).ToList();

        await _timeService.MeasureTimeAsync(async () =>
        {
            foreach (var league in leagueEntities)
            {
                if (!await LeagueExistsAsync(league))
                {
                    _context.Leagues.Add(league);
                }
            }
            await _context.SaveChangesAsync();
            return Task.CompletedTask;
        }, elapsed =>
        {
            _logger.LogInformation($"Database save executed in {elapsed.TotalMilliseconds} ms");
        });
    }
        
    private async Task<bool> LeagueExistsAsync(LeagueEntity leagueEntity)
    {
        return await _context.Leagues.AnyAsync(l =>
            l.Name == leagueEntity.Name &&
            l.Country == leagueEntity.Country &&
            l.Season == leagueEntity.Season);
    }
}