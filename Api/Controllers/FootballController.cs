using Microsoft.AspNetCore.Mvc;
using Services.Football;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FootballController : ControllerBase
{
    private readonly IFootballService _footballService;

    public FootballController(IFootballService footballService)
    {
        _footballService = footballService;
    }

    [HttpGet("leagues")]
    public async Task<IActionResult> GetLeagues(int id)
    {
        var result = await _footballService.GetLeaguesAsync(id);
        return Ok(result);
    }
}