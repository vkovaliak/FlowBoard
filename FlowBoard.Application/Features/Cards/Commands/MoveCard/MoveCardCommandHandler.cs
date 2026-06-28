using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
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