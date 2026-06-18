using FlowBoard.Application.Features.Labels.Commands.AttachLabel;
using FlowBoard.Application.Features.Labels.Commands.CreateLabel;
using FlowBoard.Application.Features.Labels.Commands.DeleteLabel;
using FlowBoard.Application.Features.Labels.Commands.DetachLabel;
using FlowBoard.Application.Features.Labels.Commands.UpdateLabel;
using FlowBoard.Application.Features.Labels.Queries.GetBoardLabels;
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
public class LabelsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<BoardHub> _hubContext;

    public LabelsController(IMediator mediator, IHubContext<BoardHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost("labels")]
    public async Task<IActionResult> CreateAsync(
        Guid boardId, CreateLabelCommand command)
    {
        var updatedCommand = command with { BoardId = boardId };
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPut("labels/{labelId:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid boardId, Guid labelId, UpdateLabelCommand command)
    {
        var updatedCommand = command with 
        { 
            BoardId = boardId, 
            LabelId = labelId 
        };

        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpDelete("labels/{labelId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid boardId, Guid labelId)
    {
        var command = new DeleteLabelCommand(boardId, labelId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPost("cards/{cardId:guid}/labels/{labelId:guid}")]
    public async Task<IActionResult> AttachAsync(
        Guid boardId, Guid cardId, Guid labelId)
    {
        var command = new AttachLabelCommand(boardId, cardId, labelId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpDelete("cards/{cardId:guid}/labels/{labelId:guid}")]
    public async Task<IActionResult> DetachAsync(
        Guid boardId, Guid cardId, Guid labelId)
    {
        var command = new DetachLabelCommand(boardId, cardId, labelId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpGet("labels")]
    public async Task<IActionResult> GetBoardLabelsAsync(Guid boardId)
    {
        var query = new GetBoardLabelsQuery(boardId);
        var result = await _mediator.Send(query);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}