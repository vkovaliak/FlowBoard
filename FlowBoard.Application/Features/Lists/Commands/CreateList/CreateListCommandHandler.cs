using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.CreateList;

public class CreateListCommandHandler : IRequestHandler<CreateListCommand, Guid>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    
    public CreateListCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }
    
    public async Task<Guid> Handle(CreateListCommand command, CancellationToken cancellationToken)
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

            var position = await uow.ListRepository.GetNextPositionAsync(command.BoardId);

            var list = new ListEntity
            {
                Id = Guid.NewGuid(),
                BoardId = command.BoardId,
                Name = command.Name,
                Position = position,
                CreatedBy = command.CurrentUserId
            };

            await uow.ListRepository.CreateAsync(list);

            uow.Commit();
            return list.Id;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}