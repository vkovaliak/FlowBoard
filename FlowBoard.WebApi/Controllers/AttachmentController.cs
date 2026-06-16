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
[Route("api/attachment")]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<CommentHub> _commentHubContext;

    public AttachmentController(IMediator mediator, IHubContext<CommentHub> commentHubContext)
    {
        _mediator = mediator;
        _commentHubContext = commentHubContext;
    }

    [HttpPost("card/{cardId:guid}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCardAttachmentAsync(
        Guid cardId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is missing or empty.");
        }

        using var stream = file.OpenReadStream();

        var command = new UploadCardAttachmentCommand(
            cardId, stream, file.FileName);

        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _commentHubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CardAttachmentsChanged, cardId);

        return Ok(result.Value);
    }

    [HttpPost("card/{cardId:guid}/comment/{commentId:guid}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCommentAttachmentAsync(
        Guid cardId, Guid commentId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is missing or empty.");
        }

        using var stream = file.OpenReadStream();

        var command = new UploadCommentAttachmentCommand(
            commentId, stream, file.FileName);

        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _commentHubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, result.Value);

        return Ok(result.Value);
    }

    [HttpDelete("card/{cardId:guid}/attachment/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteCardAttachmentAsync(Guid cardId, Guid attachmentId)
    {
        var command = new DeleteCardAttachmentCommand(attachmentId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _commentHubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CardAttachmentsChanged, cardId);

        return Ok(result);
    }

    [HttpDelete("card/{cardId:guid}/comment/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteCommentAttachmentAsync(Guid cardId, Guid attachmentId)
    {
        var command = new DeleteCommentAttachmentCommand(attachmentId);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        await _commentHubContext.Clients.Group(cardId.ToString())
            .SendAsync(HubMethods.CommentUpdated, attachmentId);

        return Ok(result);
    }
}