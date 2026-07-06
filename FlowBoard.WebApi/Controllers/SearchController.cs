using FlowBoard.Application.Features.Search.Queries.GlobalSearch;
using FlowBoard.Application.Features.Search.Queries.SearchUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] string query)
    {
        var result = await _mediator.Send(new GlobalSearchQuery(query));

        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> SearchUsersAsync([FromQuery] string query)
    {
        var result = await _mediator.Send(new SearchUsersQuery(query));
        return Ok(result);
    }
}