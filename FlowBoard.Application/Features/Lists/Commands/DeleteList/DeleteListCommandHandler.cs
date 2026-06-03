using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.DeleteList;

public class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public DeleteListCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<bool>> Handle(DeleteListCommand command, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(command.BoardId);
            if (board is null)
            {
                return Result.Fail("Board not found");
            }
            
            var isMember = await uow.BoardRepository.IsMemberAsync(command.BoardId, command.CurrentUserId);
            if (!isMember && board.CreatedBy != command.CurrentUserId)
            {
                return Result.Fail("You don't have access to this board");
            }

            var list = await uow.ListRepository.GetByIdAsync(command.ListId);
            if (list is null)
            {
                return Result.Fail("List is not found");
            }

            var result = await uow.ListRepository.DeleteAsync(list);
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