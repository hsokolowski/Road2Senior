using Microsoft.AspNetCore.Mvc;
using Services.EndpointClients;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FootballController : ControllerBase
{
    private readonly IApiFootballClient _apiFootballClient;

    public FootballController(IApiFootballClient apiFootballClient)
    {
        _apiFootballClient = apiFootballClient;
    }

    [HttpGet("league")]
    public async Task<IActionResult> GetLeague(int id)
    {
        var result = await _apiFootballClient.GetLeagueAsync(id);
        return Ok(result);
    }
}