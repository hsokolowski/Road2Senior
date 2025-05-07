using Application.Database;
using Contracts;
using Contracts.League;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace Road2Senior.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternalLeagueController : ControllerBase
{
    private readonly ILeagueRepository _leagueRepository;

    public InternalLeagueController(ILeagueRepository leagueRepository)
    {
        _leagueRepository = leagueRepository;
    }
    
    [HttpGet("league/{id}")]
    public async Task<IActionResult> GetLeague(int id)
    {
        var league = await _leagueRepository.GetLeagueAsync(id);
        if (league == null)
        {
            return NotFound("No value in DB :/");
        }
        var model = new LeagueDto()
        {
            Country = league.Country,
            Name = league.Name,
            Season = league.Season
        };

        return Ok(model);
    }
    
    [HttpPost("league")]
    public async Task<IActionResult> SaveLeagues([FromBody] List<LeagueDto> leagues)
    {
        if (leagues == null || !leagues.Any())
        {
            return BadRequest("No leagues provided.");
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            await _leagueRepository.SaveLeaguesToDatabaseAsync(leagues);
            return Ok("Save success");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}