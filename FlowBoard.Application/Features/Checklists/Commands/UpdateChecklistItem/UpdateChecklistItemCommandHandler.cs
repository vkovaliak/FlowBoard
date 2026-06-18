using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandHandler
    : IRequestHandler<UpdateChecklistItemCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateChecklistItemCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        UpdateChecklistItemCommand command, CancellationToken cancellationToken)
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

            var item = await uow.ChecklistItemRepository.GetByIdAsync(command.ItemId);
            if (item is null || item.CardId != command.CardId)
            {
                return Result.Fail("Checklist item not found");
            }

            item.Text = command.Text;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = currentUserId;

            var result = await uow.ChecklistItemRepository.UpdateAsync(item);

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