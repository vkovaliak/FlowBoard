using FlowBoard.Application.Features.Lists.Commands.CreateList;
using FlowBoard.Application.Features.Lists.Commands.DeleteList;
using FlowBoard.Application.Features.Lists.Commands.MoveList;
using FlowBoard.Application.Features.Lists.Commands.UpdateList;
using FlowBoard.Domain.Constants;
using FlowBoard.WebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/lists")]
[Authorize]
public class ListsConroller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<BoardHub> _hubContext;

    public ListsConroller(IMediator mediator, IHubContext<BoardHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateListCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(command.BoardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, command.BoardId);

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

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

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

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPut("{boardId:guid}/list/{listId:guid}/move")]
    public async Task<IActionResult> MoveAsync(Guid boardId, 
        Guid listId, MoveListCommand command)
    {
        var updatedCommand = new MoveListCommand(
            boardId, listId, command.NewPosition);
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }
}
