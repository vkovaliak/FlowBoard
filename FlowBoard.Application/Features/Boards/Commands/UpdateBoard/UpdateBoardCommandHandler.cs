using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.UpdateBoard;

public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand, Result<Guid>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUserService _currentUserService;


    public UpdateBoardCommandHandler(IBoardRepository boardRepository, ICurrentUserService currentUserService)
    {
        _boardRepository = boardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        var board = await _boardRepository.GetByIdAsync(request.BoardId);
        if (board == null)
        {
            return Result.Fail(ErrorMessages.BoardNotFound);
        }

        if (board.CreatedBy != currentUserId)
        {
            return Result.Fail("You do not have permission to update this board.");
        }

        board.Name = request.Name;
        board.IsPublic = request.IsPublic;
        board.UpdatedAt = DateTime.UtcNow;
        board.UpdatedBy = currentUserId;

        var result =  await _boardRepository.UpdateAsync(board);
        if (!result)
        {
            return Result.Fail("Failed to update the board in the database.");
        }
        
        return Result.Ok(board.Id);
    }
}