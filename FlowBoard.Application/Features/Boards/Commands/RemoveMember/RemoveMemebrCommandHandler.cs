using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Authorization;
using FlowBoard.Domain.Constants;
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
            var board = await uow.BoardRepository.GetByIdAsync(
                request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var currentUserRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);

            if (currentUserRole is null
                || !BoardPermissions.CanRemoveMembers(
                    currentUserRole.Value))
            {
                return Result.Fail(
                    "You do not have permission to remove members.");
            }

            if (request.UserId == currentUserId)
            {
                return Result.Fail(
                    "You cannot remove yourself. Use leave board instead.");
            }

            var targetRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, request.UserId);
            if (targetRole is null)
            {
                return Result.Fail(
                    "This user is not a member of the board.");
            }

            if (targetRole == BoardRole.Owner)
            {
                return Result.Fail("The board owner cannot be removed.");
            }

            if (currentUserRole == BoardRole.Admin 
                && targetRole == BoardRole.Admin)
            {
                return Result.Fail("Admins cannot remove other admins.");
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