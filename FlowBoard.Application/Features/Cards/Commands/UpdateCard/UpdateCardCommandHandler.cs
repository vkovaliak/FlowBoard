using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCard;

public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCardCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateCardCommand command, CancellationToken cancellationToken)
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
            
            var isMember = await uow.BoardRepository.IsMemberAsync(command.BoardId, currentUserId);
            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail(ErrorMessages.NoBoardAccess);
            }

            var list = await uow.ListRepository.GetByIdAsync(command.ListId);
            if (list is null || list.BoardId != command.BoardId) 
            {
                return Result.Fail(ErrorMessages.ListNotFound);
            }

            var card = await uow.CardRepository.GetByIdAsync(command.CardId);
            if (card is null)
            {
                return Result.Fail(ErrorMessages.CardNotFound);
            }

            card.Name = command.Name;
            card.Description = command.Description;
            card.DueDate = command.DueDate;
            card.UpdatedAt = DateTime.UtcNow;
            card.UpdatedBy = currentUserId;

            var result = await uow.CardRepository.UpdateAsync(card);
            uow.Commit();

            return Result.Ok(result);
        }
        catch
        {
            uow.Rollback();
            return Result.Fail("An error occurred while updating the card");
        }
    }
}