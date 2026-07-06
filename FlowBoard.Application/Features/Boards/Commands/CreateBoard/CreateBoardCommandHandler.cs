using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Authorization;
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

    public async Task<Result<Guid>> Handle(
        CreateBoardCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        using var uow = _uowFactory.Create();
        try
        {
            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var ownedCount = await uow.BoardRepository.GetOwnedBoardsCountAsync(
                currentUserId);

            var maxBoards = PlanPermissions.MaxBoards(user.SubscriptionPlan);

            if (ownedCount >= maxBoards)
            {
                return Result.Fail(
                    $"Free plan allows only {maxBoards} boards. Upgrade to Pro for unlimited boards.");
            }

            var background = user.SubscriptionPlan == SubscriptionPlan.Pro
                ? command.Background
                : null;

            var board = new Board
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                IsPublic = command.IsPublic,
                Background = background,
                CreatedBy = currentUserId
            };

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