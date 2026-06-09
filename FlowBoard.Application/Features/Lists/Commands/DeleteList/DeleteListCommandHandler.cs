using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.DeleteList;

public class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;


    public DeleteListCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteListCommand command, CancellationToken cancellationToken)
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
            
            var isMember = await uow.BoardRepository.IsMemberAsync(command.BoardId, currentUserId);
            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail("You don't have access to this board");
            }

            var list = await uow.ListRepository.GetByIdAsync(command.ListId);
            if (list is null)
            {
                return Result.Fail("List is not found");
            }

            var deletedPosition = list.Position;
            var boardId = list.BoardId;

            var result = await uow.ListRepository.DeleteAsync(list);

            if (result)
            {
                await uow.ListRepository.ShiftPositionsAfterDeleteAsync(
                    boardId, deletedPosition);
            }

            uow.Commit();

            return Result.Ok(result);
        }
        catch
        {
            uow.Rollback();
            return false;
        }
        

    }
}