using FlowBoard.Application.Features.Lists.Commands.CreateList;
using FlowBoard.Application.Features.Lists.Commands.DeleteList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/lists")]
public class ListsConroller : ControllerBase
{
    private readonly IMediator _mediator;

    public ListsConroller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateListCommand command)
    {
        var listId = await _mediator.Send(command);
        return Ok(listId);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(DeleteListCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
