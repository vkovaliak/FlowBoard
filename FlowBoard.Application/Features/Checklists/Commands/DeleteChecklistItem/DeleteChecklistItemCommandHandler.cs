using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.DeleteChecklistItem;

public class DeleteChecklistItemCommandHandler
    : IRequestHandler<DeleteChecklistItemCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public DeleteChecklistItemCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        DeleteChecklistItemCommand command, CancellationToken cancellationToken)
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

            var item = await uow.ChecklistItemRepository.GetByIdAsync(command.ItemId);
            if (item is null || item.CardId != command.CardId)
            {
                return Result.Fail("Checklist item not found");
            }

            var result = await uow.ChecklistItemRepository.DeleteAsync(item);

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
                ActionType = ActivityAction.ChecklistItemDeleted,
                Description = $"Checklist item '{item.Text}' deleted by {user.UserName}",
                CreatedAt = DateTime.UtcNow
            };

            await uow.ActivityRepository.CreateAsync(activity);

            uow.Commit();
            return Result.Ok(result);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}