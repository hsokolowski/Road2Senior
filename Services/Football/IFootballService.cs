using Domain.Football.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Domain.Entities;

namespace Services.Football
{
    public interface IFootballService
    {
        Task<IEnumerable<LeagueModel>> GetLeaguesAsync(int id);
    }
}