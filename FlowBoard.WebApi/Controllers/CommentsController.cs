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
[Route("api/comment")]
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

    [HttpPost("card/{cardId:guid}")]
    public async Task<IActionResult> CreateAsync(Guid cardId, CreateCommentCommand command)
    {
        var updatedCommand = command with { CardId = cardId };
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, result.Value);

        return Ok(result.Value);
    }

    [HttpGet("card/{cardId:guid}")]
    public async Task<IActionResult> GetByCardIdAsync(Guid cardId)
    {
        var result = await _mediator.Send(
            new GetCommentsByCardIdQuery(cardId));

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("card/{cardId:guid}/comment/{commentId:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid cardId, Guid commentId, UpdateCommentCommand command)
    {
        var updatedCommand = command with { CardId = cardId, CommentId = commentId };
        
        var result = await _mediator.Send(updatedCommand);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _hubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, commentId);

        return Ok(result.Value);
    }

    [HttpDelete("card/{cardId:guid}/comment/{commentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid cardId, Guid commentId)
    {
        var command = new DeleteCommentCommand(cardId, commentId);

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
