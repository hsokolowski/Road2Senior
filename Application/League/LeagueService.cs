// using Contracts.Cache;
//
// namespace Contracts.League
// {
//     public class LeagueService : ILeagueService
//     {
//         private readonly IApiFootballClient _apiFootballClient;
//         private readonly ICacheService _cacheService;
//
//         public LeagueService(IApiFootballClient apiFootballClient, ICacheService cacheService)
//         {
//             _apiFootballClient = apiFootballClient;
//             _cacheService = cacheService;
//         }
//
//         public async Task<IEnumerable<LeagueDto>> GetLeaguesAsync(int id)
//         {
//             return await _cacheService.GetOrAddAsync($"leagues{id}", async () =>
//             {
//                 var apiResponse = await _apiFootballClient.GetLeagueAsync(id);
//                 List<LeagueDto> leagueModels = new List<LeagueDto>();
//                 
//                 foreach (var item in apiResponse.Response)
//                 {
//                     leagueModels.Add(new LeagueDto()
//                     {
//                         Country = item.Country.Name,
//                         Name = item.League.Name,
//                         Season = item.Seasons.FirstOrDefault(s => s.IsCurrent)?.Year ?? item.Seasons.First().Year
//                     });
//                 }
//                
//                 return leagueModels;
//             }, TimeSpan.FromHours(1));
//         }
//
//         
//
//         
//     }
// }