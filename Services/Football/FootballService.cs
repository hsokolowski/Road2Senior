using Contracts;
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

        public async Task<IEnumerable<LeagueModel>> GetLeaguesAsync(int id)
        {
            return await _cacheService.GetOrAddAsync("leagues", async () =>
            {
                var apiResponse = await _apiFootballClient.GetLeagueAsync(id);
                List<LeagueModel> leagueModels = new List<LeagueModel>();
                
                foreach (var item in apiResponse.Response)
                {
                    leagueModels.Add(new LeagueModel()
                    {
                        Country = item.Country.Name,
                        Name = item.League.Name,
                        Season = item.Seasons.FirstOrDefault(s => s.IsCurrent)?.Year ?? item.Seasons.First().Year
                    });
                }
               
                return leagueModels;
            }, TimeSpan.FromHours(1));
        }

        

        
    }
}