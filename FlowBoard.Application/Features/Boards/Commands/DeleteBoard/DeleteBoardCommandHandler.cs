using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, Result<bool>>
{
    private readonly IBoardRepository _boardRepository;

    public DeleteBoardCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<Result<bool>> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Result.Fail("Board not found.");
        }

        if (board.CreatedBy != request.CurrentUserId)
        {
            return Result.Fail("You do not have permission to delete this board.");
        }

        var result = await _boardRepository.DeleteAsync(board);
        if (!result)
        {
            return Result.Fail("Failed to delete the board from the database.");
        }

        return Result.Ok(result);
    }
}