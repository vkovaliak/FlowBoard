using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.InviteMember;

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, bool>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public InviteMemberCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<bool> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board == null)
            {
                return false;
            }

            if (board.CreatedBy != request.CurrentUserId)
            {
                return false;
            }

            var userToInvite = await uow.UserRepository.GetByEmailAsync(request.Email);
            if (userToInvite == null)
            {
                return false;
            }

            var isAlreadyMember = await uow.BoardRepository.IsMemberAsync(board.Id, userToInvite.Id);
            if (isAlreadyMember)
            {
                return false;
            }

            await uow.BoardRepository.AddMemberAsync(board.Id, userToInvite.Id);

            uow.Commit();
            return true;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
        
    }
}