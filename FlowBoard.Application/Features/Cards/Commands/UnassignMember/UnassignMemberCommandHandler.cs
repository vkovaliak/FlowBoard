using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UnassignMember;

public class UnassignMemberCommandHandler 
    : IRequestHandler<UnassignMemberCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UnassignMemberCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        UnassignMemberCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(command.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var isMember = await uow.BoardRepository.IsMemberAsync(
                command.BoardId, currentUserId);
            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail(ErrorMessages.NoBoardAccess);
            }

            await uow.CardAssigneeRepository.UnassignAsync(
                command.CardId, command.UserId);
            
            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var unassignedUser = await uow.UserRepository.GetByIdAsync(command.UserId);
            if (unassignedUser is null)
            {
                return Result.Fail("User is not found");
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                CardId = command.CardId,
                BoardId = command.BoardId,
                UserId = currentUserId,
                ActionType = ActivityAction.AssigneeRemoved,
                Description = $"{user.UserName} unassigned {unassignedUser.UserName} from card",
                CreatedAt = DateTime.UtcNow
            };

            await uow.ActivityRepository.CreateAsync(activity);

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