using FlowBoard.Application.Features.Comments.Commands.CreateComment;
using FlowBoard.Application.Features.Comments.Queries.GetCommentsByCardId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Route("api/comment")]
[Authorize]
public class CommentConroller : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentConroller(IMediator mediator)
    {
        _mediator = mediator;
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
}
