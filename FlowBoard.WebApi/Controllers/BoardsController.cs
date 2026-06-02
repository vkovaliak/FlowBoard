using FlowBoard.Application.Features.Boards.Commands.CreateBoard;
using FlowBoard.Application.Features.Boards.Commands.DeleteBoard;
using FlowBoard.Application.Features.Boards.Commands.UpdateBoard;
using FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;
using FlowBoard.Application.Features.Boards.Queries.GetMyBoards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebAPI.Controllers;

[ApiController]
[Route("api/boards")]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateBoardCommand command)
    {
        var boardId = await _mediator.Send(command);

        return Ok(boardId);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyBoardsAsync(
        [FromHeader(Name = "X-Current-User-Id")] Guid currentUserId)
    {
        var query = new GetMyBoardsQuery(currentUserId);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetailsAsync(
        [FromRoute] Guid id)
    {
        var query = new GetBoardDetailsQuery(id);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(UpdateBoardCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(DeleteBoardCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
