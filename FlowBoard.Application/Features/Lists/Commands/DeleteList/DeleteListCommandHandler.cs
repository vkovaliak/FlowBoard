using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.DeleteList;

public class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, bool>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public DeleteListCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<bool> Handle(DeleteListCommand command, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(command.BoardId) ?? throw new KeyNotFoundException ("Board not found");
            var isMember = await uow.BoardRepository.IsMemberAsync(command.BoardId, command.CurrentUserId);

            if (!isMember && board.CreatedBy != command.CurrentUserId)
            {
                throw new UnauthorizedAccessException("You don't have access to this board");
            }

            var list = await uow.ListRepository.GetByIdAsync(command.ListId) ?? throw new KeyNotFoundException("List not found");

            var result = await uow.ListRepository.DeleteAsync(list);
            uow.Commit();
            return result;
        }
        catch
        {
            uow.Rollback();
            return false;
        }
        

    }
}