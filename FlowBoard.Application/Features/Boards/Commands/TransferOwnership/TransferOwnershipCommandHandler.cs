using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Authorization;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.TransferOwnership;

public class TransferOwnershipCommandHandler
    : IRequestHandler<TransferOwnershipCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public TransferOwnershipCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        TransferOwnershipCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        if (request.NewOwnerId == currentUserId)
        {
            return Result.Fail("You are already the owner of this board.");
        }

        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var currentRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);

            if (currentRole is null
                || !BoardPermissions.CanTransferOwnership(currentRole.Value))
            {
                return Result.Fail("Only the owner can transfer ownership.");
            }

            var targetRole = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, request.NewOwnerId);

            if (targetRole is null)
            {
                return Result.Fail(
                    "The selected user is not a member of this board.");
            }

            await uow.BoardRepository.TransferOwnershipAsync(
                board.Id, currentUserId, request.NewOwnerId);

            uow.Commit();
            return Result.Ok(true);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}