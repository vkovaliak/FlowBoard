using FlowBoard.Application.Features.Comments.Commands.CreateComment;
using FlowBoard.Application.Features.Comments.Commands.DeleteComment;
using FlowBoard.Application.Features.Comments.Commands.UpdateComment;
using FlowBoard.Application.Features.Comments.Queries.GetCommentsByCardId;
using FlowBoard.Domain.Constants;
using FlowBoard.WebApi.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/boards/{boardId:guid}/cards/{cardId:guid}/comments")]
[Authorize]
public class CommentConroller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<CommentHub> _hubContext;

    public CommentConroller(
        IMediator mediator, IHubContext<CommentHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid boardId, Guid cardId, CreateCommentCommand command)
    {
        var updatedCommand = command with { BoardId = boardId, CardId = cardId };
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, result.Value);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCardIdAsync(Guid boardId, Guid cardId)
    {
        var result = await _mediator.Send(
            new GetCommentsByCardIdQuery(cardId));

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("{commentId:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid boardId, Guid cardId, Guid commentId, UpdateCommentCommand command)
    {
        var updatedCommand = command with { BoardId = boardId, CardId = cardId, CommentId = commentId };
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, commentId);

        return Ok(result.Value);
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid boardId, Guid cardId, Guid commentId)
    {
        var command = new DeleteCommentCommand(boardId, cardId, commentId);

        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, commentId);

        return Ok(result.Value);
    }
}
