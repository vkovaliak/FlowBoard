using FlowBoard.Application.Abstractions;
using FlowBoard.Application.Features.Activities.Events;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;


    public CreateCardCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService, IMediator mediator)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateCardCommand command, CancellationToken cancellationToken)
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

            var position = await uow.CardRepository.GetNextPositionAsync(command.ListId);

            var card = new Card
            {
                Id = Guid.NewGuid(),
                ListId = command.ListId,
                Name = command.Name,
                Description = command.Description,
                Position = position,
                CreatedBy = currentUserId,
            };

            await uow.CardRepository.CreateAsync(card);

            uow.Commit();

            await _mediator.Publish(new BoardActivityEvent(
                BoardId: command.BoardId,
                UserId: currentUserId,
                ActionType: ActivityAction.CardCreated,
                EntityType: ActivityEntityType.Card,
                EntityId: card.Id,
                Description: $"created card '{command.Name}'"));
            
            return Result.Ok(card.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}