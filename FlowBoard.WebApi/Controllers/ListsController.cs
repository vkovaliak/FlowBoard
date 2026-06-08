using FlowBoard.Application.Features.Lists.Commands.CreateList;
using FlowBoard.Application.Features.Lists.Commands.DeleteList;
using FlowBoard.Application.Features.Lists.Commands.UpdateList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/lists")]
[Authorize]
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
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("{boardId:guid}/list/{listId:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid boardId, 
        Guid listId, 
        UpdateListCommand command)
    {
        var updatedCommand = command with { BoardId = boardId, ListId = listId };
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{boardId:guid}/list/{listId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid boardId, Guid listId)
    {
        var command = new DeleteListCommand(ListId: listId, BoardId: boardId);
        
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}
