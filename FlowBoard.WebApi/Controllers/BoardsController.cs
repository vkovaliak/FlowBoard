using FlowBoard.Application.Features.Boards.Commands.CreateBoard;
using FlowBoard.Application.Features.Boards.Commands.DeleteBoard;
using FlowBoard.Application.Features.Boards.Commands.UpdateBoard;
using FlowBoard.Application.Features.Boards.Commands.InviteMember;
using FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;
using FlowBoard.Application.Features.Boards.Queries.GetMyBoards;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using FlowBoard.WebApi.Hubs;
using FlowBoard.Domain.Constants;
using FlowBoard.Application.Features.Boards.Commands.RemoveMember;
using FlowBoard.Application.Features.Boards.Commands.LeaveBoard;
using FlowBoard.Application.Features.Boards.Commands.ToggleFavorite;
using FlowBoard.Application.Features.Boards.Commands.ArchiveBoard;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/boards")]
[Authorize]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<BoardHub> _hubContext;

    public BoardsController(IMediator mediator, IHubContext<BoardHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateBoardCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyBoardsAsync()
    {
        var query = new GetMyBoardsQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetailsAsync(Guid id)
    {
        var query = new GetBoardDetailsQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateBoardCommand command)
    {
        var updatedCommand = command with { BoardId = id };
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(id.ToString())
            .SendAsync(HubMethods.BoardUpdated, id);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var command = new DeleteBoardCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(id.ToString())
            .SendAsync(HubMethods.BoardUpdated, id);

        return Ok(result.Value);
    }

    [HttpPost("{boardId:guid}/invite")]
    public async Task<IActionResult> InviteMemberAsync(Guid boardId, InviteMemberCommand command)
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

    [HttpDelete("{boardId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMemberAsync(Guid boardId, Guid userId)
    {
        var command = new RemoveMemberCommand(boardId, userId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpDelete("{boardId:guid}/leave")]
    public async Task<IActionResult> LeaveBoardAsync(Guid boardId)
    {
        var command = new LeaveBoardCommand(boardId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPut("{boardId:guid}/favorite")]
    public async Task<IActionResult> ToggleFavoriteAsync(Guid boardId)
    {
        var command = new ToggleFavoriteCommand(boardId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPatch("{boardId:guid}/archive")]
    public async Task<IActionResult> ArchiveBoardAsync(Guid boardId)
    {
        var command = new ArchiveBoardCommand(boardId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}
