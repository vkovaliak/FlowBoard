using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.InviteMember;

public class InviteMemberCommandHandler : IRequestHandler<InviteMemberCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public InviteMemberCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<bool>> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail("Board not found.");
            }

            var userToInvite = await uow.UserRepository.GetByEmailAsync(request.Email);
            if (userToInvite == null)
            {
                return Result.Fail("User with this email does not exist.");
            }

            var isAlreadyMember = await uow.BoardRepository.IsMemberAsync(board.Id, userToInvite.Id);
            if (isAlreadyMember)
            {
                return Result.Fail("This user is already a member of this board.");
            }

            await uow.BoardRepository.AddMemberAsync(board.Id, userToInvite.Id);

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