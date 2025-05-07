using Application.External;
using Contracts;
using Contracts.Cache;
using Contracts.League;
using MediatR;

namespace Application.League.Queries;

public class GetLeaguesQueryHandler : IRequestHandler<GetLeaguesQuery, IEnumerable<LeagueDto>>
{
    private readonly IApiFootballClient _apiFootballClient;
    private readonly ICacheService _cacheService;

    public GetLeaguesQueryHandler(IApiFootballClient apiFootballClient, ICacheService cacheService)
    {
        _apiFootballClient = apiFootballClient;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<LeagueDto>> Handle(GetLeaguesQuery request, CancellationToken cancellationToken)
    {
        int id = request.LeagueId;
        return await _cacheService.GetOrAddAsync($"leagues{id}", async () =>
        {
            var apiResponse = await _apiFootballClient.GetLeagueAsync(id);
            List<LeagueDto> leagueModels = new();

            foreach (var item in apiResponse.Response)
            {
                leagueModels.Add(new LeagueDto
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