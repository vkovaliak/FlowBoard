using FlowBoard.Application.Features.Boards.Commands.CreateBoard;
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
    public async Task<IActionResult> CreateBoard(CreateBoardCommand command)
    {
        var boardId = await _mediator.Send(command);

        return Ok(boardId);
    }
}
