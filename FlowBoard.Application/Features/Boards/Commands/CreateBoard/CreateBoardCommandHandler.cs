using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
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
            await uow.BoardRepository.CreateAsync(board);

            await uow.BoardRepository.AddMemberAsync(board.Id, command.CurrentUserId);

            for (int i = 0; i < BoardConstants.DefaultColumnNames.Length; i++)
            {
                var defaultList = new ListEntity
                {
                    Id = Guid.NewGuid(),
                    BoardId = board.Id,
                    Name = BoardConstants.DefaultColumnNames[i],
                    Position = i,
                    CreatedBy = command.CurrentUserId
                };

                await uow.ListRepository.CreateAsync(defaultList);
            }

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