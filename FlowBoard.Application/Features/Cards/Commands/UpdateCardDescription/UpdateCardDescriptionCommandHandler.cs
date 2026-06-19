using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCardDescription;

public class UpdateCardDescriptionommandHandler
    : IRequestHandler<UpdateCardDescriptionCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCardDescriptionommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }


    public async Task<Result<bool>> Handle(
        UpdateCardDescriptionCommand command, CancellationToken cancellationToken)
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

            var card = await uow.CardRepository.GetByIdAsync(command.CardId);
            if (card is null)
            {
                return Result.Fail("Card not found");
            }

            card.Description = command.Description;
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
