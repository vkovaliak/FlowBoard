    using FlowBoard.Application.Features.Boards.Commands.CreateBoard;
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
        public async Task<IActionResult> GetMyBoards(
            [FromHeader(Name = "X-Current-User-Id")] Guid currentUserId)
        {
            var query = new GetMyBoardsQuery(currentUserId);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
    }
