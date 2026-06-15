using FlowBoard.Application.Features.Attachments.Commands.UploadCardAttachment;
using FlowBoard.Application.Features.Attachments.Commands.UploadCommentAttachment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/attachment")]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttachmentController(IMediator mediator)
    {
        _mediator = mediator;
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

        return Ok(result.Value);
    }

    [HttpPost("comment/{commentId:guid}/upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCommentAttachmentAsync(
        Guid commentId, IFormFile file)
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

        return Ok(result.Value);
    }
}