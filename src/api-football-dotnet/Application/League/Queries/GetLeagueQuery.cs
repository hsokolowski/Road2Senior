using Contracts;
using Contracts.League;
using MediatR;

namespace Application.League.Queries;

public record GetLeaguesQuery(int LeagueId) : IRequest<IEnumerable<LeagueDto>>;