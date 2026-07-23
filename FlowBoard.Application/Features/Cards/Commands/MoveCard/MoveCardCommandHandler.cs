using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.MoveCard;

public class MoveCardCommandHandler : IRequestHandler<MoveCardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public MoveCardCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(MoveCardCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        
        try
        {
            var card = await uow.CardRepository.GetByIdAsync(request.CardId);
            if (card == null)
            {
                return Result.Fail(ErrorMessages.CardNotFound);
            }

            var targetList = await uow.ListRepository.GetByIdAsync(request.NewListId);
            if (targetList == null || targetList.BoardId != request.BoardId)
            {
                return Result.Fail("Target list not found or access denied.");
            }

            Guid oldListId = card.ListId;
            int oldPosition = card.Position;
            Guid newListId = request.NewListId;
            int newPosition = request.NewPosition;

            if (oldListId == newListId && oldPosition == newPosition)
            {
                return Result.Ok(true);
            }

            await uow.CardRepository.ShiftPositionsOnMoveAsync(
                oldListId, newListId, oldPosition, newPosition);

            card.ListId = newListId;
            card.Position = newPosition;
            card.UpdatedAt = DateTime.UtcNow;
            card.UpdatedBy = currentUserId;

            await uow.CardRepository.UpdateAsync(card);

            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var oldList = await uow.ListRepository.GetByIdAsync(oldListId);
            if (oldList is null)
            {
                return Result.Fail("List is not found");
            }

            var newList = await uow.ListRepository.GetByIdAsync(newListId);
            if (newList is null)
            {
                return Result.Fail("List is not found");
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                BoardId = request.BoardId,
                UserId = currentUserId,
                ActionType = ActivityAction.CardMoved,
                Description = $"Card moved from {oldList.Name} to {newList.Name} by {user.UserName}",
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