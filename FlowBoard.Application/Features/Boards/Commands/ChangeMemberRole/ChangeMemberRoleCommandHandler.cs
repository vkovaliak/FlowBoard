using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Authorization;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ChangeMemberRole;

public class ChangeMemberRoleCommandHandler
    : IRequestHandler<ChangeMemberRoleCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public ChangeMemberRoleCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        ChangeMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        if (request.NewRole == BoardRole.Owner)
        {
            return Result.Fail(
                "Use transfer ownership to assign the owner role.");
        }

        if (request.UserId == currentUserId)
        {
            return Result.Fail("You cannot change your own role.");
        }

        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var actorRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);
            if (actorRole is null)
            {
                return Result.Fail("You are not a member of this board.");
            }

            if (!BoardPermissions.CanAssignRole(actorRole.Value, request.NewRole))
            {
                return Result.Fail(
                    "You do not have permission to assign this role.");
            }

            var targetRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, request.UserId);
            if (targetRole is null)
            {
                return Result.Fail("This user is not a member of the board.");
            }

            if (actorRole == BoardRole.Admin
                && targetRole is BoardRole.Admin or BoardRole.Owner)
            {
                return Result.Fail(
                    "Admins cannot modify other admins or the owner.");
            }

            var updated = await uow.BoardRepository.UpdateMemberRoleAsync(
                board.Id, request.UserId, request.NewRole);
            if (!updated)
            {
                return Result.Fail("Failed to update the member role.");
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