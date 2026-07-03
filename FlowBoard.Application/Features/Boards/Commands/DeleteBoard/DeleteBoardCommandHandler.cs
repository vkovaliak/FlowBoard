using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, Result<bool>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUserService _currentUserService;


    public DeleteBoardCommandHandler(IBoardRepository boardRepository, ICurrentUserService currentUserService)
    {
        _boardRepository = boardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        var board = await _boardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Result.Fail(ErrorMessages.BoardNotFound);
        }

        var role = await _boardRepository.GetUserRoleAsync(
            board.Id, currentUserId);
        if (role != BoardRole.Owner)
        {
            return Result.Fail("Only the board owner can delete this board.");
        }

        var result = await _boardRepository.DeleteAsync(board);
        if (!result)
        {
            return Result.Fail("Failed to delete the board from the database.");
        }

        return Result.Ok(result);
    }
}