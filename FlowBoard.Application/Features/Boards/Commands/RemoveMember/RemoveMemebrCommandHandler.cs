using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.RemoveMember;

public class RemoveMemberCommandHandler
    : IRequestHandler<RemoveMemberCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public RemoveMemberCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        RemoveMemberCommand request, CancellationToken cancellationToken)
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
            if (currentUserRole != BoardRole.Owner)
            {
                return Result.Fail("Only the board owner can remove members.");
            }

            if (request.UserId == currentUserId)
            {
                return Result.Fail("Owner cannot remove themselves from the board.");
            }

            var isMember = await uow.BoardRepository.IsMemberAsync(
                board.Id, request.UserId);
            if (!isMember)
            {
                return Result.Fail("This user is not a member of the board.");
            }

            var removed = await uow.BoardRepository.RemoveMemberAsync(
                board.Id, request.UserId);
            if (!removed)
            {
                return Result.Fail("Failed to remove the member.");
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