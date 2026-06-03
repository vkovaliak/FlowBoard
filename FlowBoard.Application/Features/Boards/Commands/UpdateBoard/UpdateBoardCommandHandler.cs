using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.UpdateBoard;

public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand, Result<bool>>
{
    private readonly IBoardRepository _boardRepository;

    public UpdateBoardCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<Result<bool>> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.BoardId);
        if (board == null)
        {
            return Result.Fail("Board not found.");
        }

        if (board.CreatedBy != request.CurrentUserId)
        {
            return Result.Fail("You do not have permission to update this board.");
        }

        board.Name = request.Name;
        board.IsPublic = request.IsPublic;

        var result =  await _boardRepository.UpdateAsync(board);
        if (!result)
        {
            return Result.Fail("Failed to update the board in the database.");
        }
        
        return Result.Ok(result);
    }
}