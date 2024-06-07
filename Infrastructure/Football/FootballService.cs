using Domain.Entities;
using Domain.Football.Responses;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Cache;
using Services.EndpointClients;
using Services.Football;
using Services.Timing;

namespace Infrastructure.Football
{
    public class FootballService : IFootballService
    {
        private readonly IApiFootballClient _apiFootballClient;
        private readonly FootballContext _context;
        private readonly ICacheService _cacheService;
        private readonly ITimeService _timeService;
        private readonly ILogger<FootballService> _logger;

        public FootballService(IApiFootballClient apiFootballClient, FootballContext context, ICacheService cacheService, ITimeService timeService, ILogger<FootballService> logger)
        {
            _apiFootballClient = apiFootballClient;
            _context = context;
            _cacheService = cacheService;
            _timeService = timeService;
            _logger = logger;
        }

        public async Task<IEnumerable<LeagueResponse>> GetLeaguesAsync(int id)
        {
            return await _cacheService.GetOrAddAsync("leagues", async () =>
            {
                var apiResponse = await _apiFootballClient.GetLeagueAsync(id);
                var leagues = apiResponse.Response.ToList();

                return leagues;
            }, TimeSpan.FromHours(1));
        }

        public async Task<LeagueEntity> GetLeagueAsync(int id)
        {
            return await _context.Leagues.FindAsync(id);
        }

        public async Task SaveLeaguesToDatabaseAsync(IEnumerable<LeagueResponse> leagues)
        {
            var leagueEntities = leagues.Select(l => new LeagueEntity
            {
                Name = l.Get.Name,
                Country = l.Country.Name,
                Season = l.Seasons.FirstOrDefault(s => s.IsCurrent)?.Year ?? l.Seasons.First().Year
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
}