using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.RestoreBoard;

public class RestoreBoardCommandHandler
    : IRequestHandler<RestoreBoardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IArchiveMessagePublisher _publisher;

    public RestoreBoardCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService,
        IArchiveMessagePublisher publisher)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
        _publisher = publisher;
    }

    public async Task<Result<bool>> Handle(
        RestoreBoardCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();

        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(
                request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var role = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);
            if (role != BoardRole.Owner)
            {
                return Result.Fail(
                    "Only the board owner can restore this board.");
            }

            if (board.ArchiveStatus != ArchiveStatus.Completed)
            {
                return Result.Fail("This board is not archived.");
            }

            board.ArchiveStatus = ArchiveStatus.Restoring;
            await uow.BoardRepository.UpdateAsync(board);
            uow.Commit();

            await _publisher.PublishRestoreMessageAsync(board.Id);

            return Result.Ok(true);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}