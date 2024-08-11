using Contracts;
using Domain.Football.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.DatabaseService;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public DatabaseController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    [HttpGet("league/{id}")]
    public async Task<IActionResult> GetLeague(int id)
    {
        var league = await _databaseService.GetLeagueAsync(id);
        if (league == null)
        {
            return NotFound("No value in DB :/");
        }
        var model = new LeagueModel()
        {
            Country = league.Country,
            Name = league.Name,
            Season = league.Season
        };

        return Ok(model);
    }
    
    [HttpPost("league")]
    public async Task<IActionResult> SaveLeagues([FromBody] List<LeagueModel> leagues)
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
            await _databaseService.SaveLeaguesToDatabaseAsync(leagues);
            return Ok("Save success");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}