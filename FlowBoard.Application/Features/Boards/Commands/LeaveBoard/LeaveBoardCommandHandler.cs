using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.LeaveBoard;

public class LeaveBoardCommandHandler
    : IRequestHandler<LeaveBoardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public LeaveBoardCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        LeaveBoardCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail("Board not found.");
            }

            var currentUserRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);

            if (currentUserRole is null)
            {
                return Result.Fail("You are not a member of this board.");
            }

            if (currentUserRole == BoardRole.Owner)
            {
                return Result.Fail(
                    "The owner cannot leave the board. Delete it instead.");
            }

            var removed = await uow.BoardRepository.RemoveMemberAsync(
                board.Id, currentUserId);
            if (!removed)
            {
                return Result.Fail("Failed to leave the board.");
            }

            uow.Commit();
            return Result.Ok(true);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}