using FlowBoard.Application.Features.Attachments.Commands.DeleteCardAttachment;
using FlowBoard.Application.Features.Attachments.Commands.DeleteCommentAttachment;
using FlowBoard.Application.Features.Attachments.Commands.UploadCardAttachment;
using FlowBoard.Application.Features.Attachments.Commands.UploadCommentAttachment;
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
public class AttachmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<BoardHub> _boardHubContext;
    private readonly IHubContext<CommentHub> _commentHubContext;

    public AttachmentController(
        IMediator mediator,
        IHubContext<BoardHub> boardHubContext,
        IHubContext<CommentHub> commentHubContext)
    {
        _mediator = mediator;
        _boardHubContext = boardHubContext;
        _commentHubContext = commentHubContext;
    }

    [HttpPost("card/{cardId:guid}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCardAttachmentAsync(
        Guid boardId, Guid cardId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is missing or empty.");
        }

        using var stream = file.OpenReadStream();

        var command = new UploadCardAttachmentCommand(
            boardId, cardId, stream, file.FileName);

        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _boardHubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpPost("card/{cardId:guid}/comment/{commentId:guid}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCommentAttachmentAsync(
        Guid boardId, Guid cardId, Guid commentId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is missing or empty.");
        }

        using var stream = file.OpenReadStream();

        var command = new UploadCommentAttachmentCommand(
            boardId, commentId, stream, file.FileName);

        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _commentHubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, boardId);

        return Ok(result.Value);
    }

    [HttpDelete("card/{cardId:guid}/attachment/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteCardAttachmentAsync(
        Guid boardId, Guid cardId, Guid attachmentId)
    {
        var command = new DeleteCardAttachmentCommand(
            boardId, attachmentId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _boardHubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.BoardUpdated, boardId);

        return Ok(result);
    }

    [HttpDelete("card/{cardId:guid}/comment/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteCommentAttachmentAsync(
        Guid boardId, Guid cardId, Guid attachmentId)
    {
        var command = new DeleteCommentAttachmentCommand(
            boardId, attachmentId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _commentHubContext.Clients.Group(boardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, boardId);

        return Ok(result);
    }
}