using Application.League.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Road2Senior.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalLeagueController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExternalLeagueController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("leagues")]
    public async Task<IActionResult> GetLeagues(int id)
    {
        var result = await _mediator.Send(new GetLeaguesQuery(id));
        return Ok(result);
    }
}