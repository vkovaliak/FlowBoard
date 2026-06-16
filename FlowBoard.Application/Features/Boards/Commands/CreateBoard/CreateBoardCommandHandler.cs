using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.CreateBoard;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public CreateBoardCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateBoardCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            IsPublic = command.IsPublic,
            CreatedBy = currentUserId
        };

        using var uow = _uowFactory.Create();
        try
        {
            await uow.BoardRepository.CreateAsync(board);

            await uow.BoardRepository.AddMemberAsync(
                board.Id, currentUserId, BoardRole.Owner);

            for (int i = 0; i < BoardConstants.DefaultColumnNames.Length; i++)
            {
                var defaultList = new ListEntity
                {
                    Id = Guid.NewGuid(),
                    BoardId = board.Id,
                    Name = BoardConstants.DefaultColumnNames[i],
                    Position = i,
                    CreatedBy = currentUserId
                };

                await uow.ListRepository.CreateAsync(defaultList);
            }

            uow.Commit();

            return Result.Ok(board.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}