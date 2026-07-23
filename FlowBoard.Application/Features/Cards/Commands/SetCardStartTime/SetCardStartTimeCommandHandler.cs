using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.SetCardStartTime;

public class SetCardStartTimeCommandHandler
    : IRequestHandler<SetCardStartTimeCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public SetCardStartTimeCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        SetCardStartTimeCommand command, CancellationToken cancellationToken)
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

            var card = await uow.CardRepository.GetByIdAsync(command.CardId);
            if (card is null)
            {
                return Result.Fail(ErrorMessages.CardNotFound);
            }

            if (command.StartTime > card.DueDate)
            {
                return Result.Fail("Start time cannot be greater than due date");
            }

            card.StartTime = command.StartTime;
            card.UpdatedAt = DateTime.UtcNow;
            card.UpdatedBy = currentUserId;

            await uow.CardRepository.UpdateAsync(card);

            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                BoardId = command.BoardId,
                UserId = currentUserId,
                ActionType = command.StartTime.HasValue 
                    ? ActivityAction.StartDateSet 
                    : ActivityAction.StartDateRemoved,
                Description = command.StartTime.HasValue
                    ? $"Start date set to {command.StartTime:d MMM yyyy} by {user.UserName}"
                    : $"Start date removed by {user.UserName}",
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