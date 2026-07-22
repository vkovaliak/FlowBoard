using FlowBoard.Application.Abstractions;
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

    public CreateCardCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        CreateCardCommand command, CancellationToken cancellationToken)
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

            var position = await uow.CardRepository.GetNextPositionAsync(
                command.ListId);

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
                ActionType = ActivityAction.CardCreated,
                Description = $"Card created by {user.UserName}",
                CreatedAt = DateTime.UtcNow
            };
            
            await uow.ActivityRepository.CreateAsync(activity);

            uow.Commit();
            
            return Result.Ok(card.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}