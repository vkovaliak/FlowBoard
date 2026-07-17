using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.DuplicateCard;

public class DuplicateCardCommandHandler
    : IRequestHandler<DuplicateCardCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public DuplicateCardCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        DuplicateCardCommand command, CancellationToken cancellationToken)
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

            var isMember = await uow.BoardRepository
                .IsMemberAsync(command.BoardId, currentUserId);

            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail(ErrorMessages.NoBoardAccess);
            }

            var original = await uow.CardRepository.GetByIdAsync(command.CardId);
            if (original is null)
            {
                return Result.Fail(ErrorMessages.CardNotFound);
            }

            var position = await uow.CardRepository
                .GetNextPositionAsync(original.ListId);

            var newCard = new Card
            {
                Id = Guid.NewGuid(),
                ListId = original.ListId,
                Name = $"{original.Name} (Copy)",
                Description = original.Description,
                DueDate = original.DueDate,
                IsCompleted = false,
                Position = position,
                CreatedBy = currentUserId
            };

            await uow.CardRepository.CreateAsync(newCard);

            await uow.CardRepository
                .CopyLabelsAsync(original.Id, newCard.Id);

            await uow.CardRepository
                .CopyAssigneesAsync(original.Id, newCard.Id);

            await uow.CardRepository
                .CopyChecklistItemsAsync(original.Id, newCard.Id, currentUserId);

            uow.Commit();
            
            return Result.Ok(newCard.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}