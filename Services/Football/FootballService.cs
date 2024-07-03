using Domain.Football.Responses;
using Infrastructure.Cache;
using Services.EndpointClients;


namespace Services.Football
{
    public class FootballService : IFootballService
    {
        private readonly IApiFootballClient _apiFootballClient;
        private readonly ICacheService _cacheService;

        public FootballService(IApiFootballClient apiFootballClient, ICacheService cacheService)
        {
            _apiFootballClient = apiFootballClient;
            _cacheService = cacheService;
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

        

        
    }
}