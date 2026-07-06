using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.UpdateBoard;

public class UpdateBoardCommandHandler 
    : IRequestHandler<UpdateBoardCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;


    public UpdateBoardCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        UpdateBoardCommand command, CancellationToken cancellationToken)
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

            var owner = await uow.UserRepository.GetByIdAsync(board.CreatedBy);
            if (owner is null)
            {
                return Result.Fail("Owner not found");
            }

            board.Name = command.Name;
            board.IsPublic = command.IsPublic;

            board.Background = owner.SubscriptionPlan == SubscriptionPlan.Pro
                ? command.Background
                : null;

            var result = await uow.BoardRepository.UpdateAsync(board);
            if (!result)
            {
                return Result.Fail("Failed to update the board in the database.");
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