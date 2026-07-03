using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Authorization;
using FlowBoard.Domain.Constants;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.InviteMember;

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public InviteMemberCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var currentUserRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);

            if (currentUserRole is null
                || !BoardPermissions.CanInviteMembers(currentUserRole.Value))
            {
                return Result.Fail("You do not have permission to invite members.");
            }

            if (!BoardPermissions.CanAssignRole(currentUserRole.Value, request.Role))
            {
                return Result.Fail("You cannot invite a member with this role.");
            }

            var userToInvite = await uow.UserRepository.GetByEmailAsync(
                request.Email);
            if (userToInvite == null)
            {
                return Result.Fail("User with this email does not exist.");
            }

            var isAlreadyMember = await uow.BoardRepository.IsMemberAsync(board.Id, userToInvite.Id);
            if (isAlreadyMember)
            {
                return Result.Fail("This user is already a member of this board.");
            }

            await uow.BoardRepository.AddMemberAsync(
                board.Id, userToInvite.Id, request.Role);

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