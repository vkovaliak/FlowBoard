using FlowBoard.Application.Features.Checklists.Commands.AddChecklistItem;
using FlowBoard.Application.Features.Checklists.Commands.DeleteChecklistItem;
using FlowBoard.Application.Features.Checklists.Commands.ToggleChecklistItem;
using FlowBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;
using FlowBoard.Domain.Constants;
using FlowBoard.WebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/boards/{boardId:guid}")]
[Authorize]
public class ChecklistController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<BoardHub> _hubContext;

    public ChecklistController(IMediator mediator, IHubContext<BoardHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost("cards/{cardId:guid}/checklist")]
    public async Task<IActionResult> AddAsync(
        Guid boardId, Guid cardId, AddChecklistItemCommand command)
    {
        var updated = command with { BoardId = boardId, CardId = cardId };
        var result = await _mediator.Send(updated);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPut("cards/{cardId:guid}/checklist/{itemId:guid}/toggle")]
    public async Task<IActionResult> ToggleAsync(
        Guid boardId, Guid cardId, Guid itemId)
    {
        var command = new ToggleChecklistItemCommand(boardId, cardId, itemId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPut("cards/{cardId:guid}/checklist/{itemId:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid boardId, Guid cardId, Guid itemId, UpdateChecklistItemCommand command)
    {
        var updated = command with 
        { 
            BoardId = boardId, 
            CardId = cardId, 
            ItemId = itemId 
        };

        var result = await _mediator.Send(updated);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpDelete("cards/{cardId:guid}/checklist/{itemId:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid boardId, Guid cardId, Guid itemId)
    {
        var command = new DeleteChecklistItemCommand(boardId, cardId, itemId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }
}