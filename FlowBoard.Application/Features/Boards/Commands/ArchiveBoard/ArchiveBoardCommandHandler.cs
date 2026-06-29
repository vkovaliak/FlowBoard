using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ArchiveBoard;

public class ArchiveBoardCommandHandler
    : IRequestHandler<ArchiveBoardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public ArchiveBoardCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        ArchiveBoardCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var role = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);
            if (role != BoardRole.Owner)
            {
                return Result.Fail("Only the board owner can archive this board.");
            }

            if (board.ArchiveStatus != ArchiveStatus.None)
            {
                return Result.Fail("This board is already archived.");
            }

            board.ArchiveStatus = ArchiveStatus.Pending;
            board.ArchivedAt = DateTime.UtcNow;

            await uow.BoardRepository.UpdateAsync(board);
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