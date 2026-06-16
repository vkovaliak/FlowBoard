using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.AssignMember;

public class AssignMemberCommandHandler 
    : IRequestHandler<AssignMemberCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public AssignMemberCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        AssignMemberCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(command.BoardId);
            if (board is null)
            {
                return Result.Fail("Board not found");
            }

            var isMember = await uow.BoardRepository.IsMemberAsync(
                command.BoardId, currentUserId);
            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail("You don't have access to this board");
            }

            var isTargetMember = await uow.BoardRepository.IsMemberAsync(
                command.BoardId, command.UserId);
            if (!isTargetMember)
            {
                return Result.Fail("User is not a member of this board");
            }

            await uow.CardAssigneeRepository.AssignAsync(
                command.CardId, command.UserId);

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