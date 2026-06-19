using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.RenameCard;

public class RenameCardCommandHandler
    : IRequestHandler<RenameCardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public RenameCardCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        RenameCardCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result.Fail("Card name cannot be empty");
        }

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

            card.Name = command.Name;
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