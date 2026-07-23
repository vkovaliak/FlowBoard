using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public class AddChecklistItemCommandHandler
    : IRequestHandler<AddChecklistItemCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public AddChecklistItemCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        AddChecklistItemCommand command, CancellationToken cancellationToken)
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

            var maxPosition = await uow.ChecklistItemRepository
                .GetMaxPositionAsync(command.CardId);

            var item = new ChecklistItem
            {
                Id = Guid.NewGuid(),
                CardId = command.CardId,
                Text = command.Text,
                IsCompleted = false,
                Position = maxPosition + 1,
                CreatedBy = currentUserId
            };

            await uow.ChecklistItemRepository.CreateAsync(item);

            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                CardId = command.CardId,
                BoardId = command.BoardId,
                UserId = currentUserId,
                ActionType = ActivityAction.ChecklistItemAdded,
                Description = $"Checklist item '{command.Text}' added by {user.UserName}",
                CreatedAt = DateTime.UtcNow
            };

            await uow.ActivityRepository.CreateAsync(activity);

            uow.Commit();
            return Result.Ok(item.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}