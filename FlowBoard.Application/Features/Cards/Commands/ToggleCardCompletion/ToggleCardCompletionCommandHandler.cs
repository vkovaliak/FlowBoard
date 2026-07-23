using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.ToggleCardCompletion;

public class ToggleCardCompletionCommandHandler
    : IRequestHandler<ToggleCardCompletionCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public ToggleCardCompletionCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        ToggleCardCompletionCommand command, CancellationToken cancellationToken)
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

            await uow.CardRepository.ToggleCompletionAsync(command.CardId);

            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var isNowCompleted = !card.IsCompleted;

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                CardId = command.CardId,
                BoardId = command.BoardId,
                UserId = currentUserId,
                ActionType = isNowCompleted 
                    ? ActivityAction.CardCompleted 
                    : ActivityAction.CardUncomleted,
                Description = isNowCompleted
                    ? $"Card completed by {user.UserName}"
                    : $"Card uncompleted by {user.UserName}",
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