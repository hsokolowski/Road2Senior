﻿using Contracts;
using Domain.Entities;
using Infrastructure.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Football;

namespace Services.DatabaseService;

public class DatabaseService : IDatabaseService
{
    private FootballContext _context;
    private readonly ITimeService _timeService;
    private readonly ILogger<FootballService> _logger;

    public DatabaseService(FootballContext context, ITimeService timeService, ILogger<FootballService> logger)
    {
        _context = context;
        _timeService = timeService;
        _logger = logger;
    }
    public async Task<LeagueEntity> GetLeagueAsync(int id)
    {
        return await _context.Leagues.FindAsync(id);
    }
    
    public async Task SaveLeaguesToDatabaseAsync(IEnumerable<LeagueModel> leagues)
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
        
    private async Task<bool> LeagueExistsAsync(LeagueEntity league)
    {
        return await _context.Leagues.AnyAsync(l =>
            l.Name == league.Name &&
            l.Country == league.Country &&
            l.Season == league.Season);
    }
}