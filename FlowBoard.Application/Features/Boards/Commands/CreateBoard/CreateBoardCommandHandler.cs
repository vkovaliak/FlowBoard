using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.CreateBoard;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateBoardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Guid> Handle(CreateBoardCommand command, CancellationToken cancellationToken)
    {
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            IsPublic = command.IsPublic,
            CreatedBy = command.CurrentUserId
        };

        using var uow = _uowFactory.Create();
        try
        {
            await uow.Boards.CreateAsync(board);

            await uow.Boards.AddMemberAsync(board.Id, command.CurrentUserId);

            uow.Commit();

            return board.Id;
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}