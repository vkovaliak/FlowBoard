using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;


    public DeleteBoardCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        try
        {
            var currentUserId = _currentUserService.GetId();
            var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
            if (board is null)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            var role = await uow.BoardRepository.GetUserRoleAsync(
                board.Id, currentUserId);
            if (role != BoardRole.Owner)
            {
                return Result.Fail("Only the board owner can restore this board.");
            }

            var result = await uow.BoardRepository.DeleteAsync(board);
            if (!result)
            {
                return Result.Fail("Failed to delete the board from the database.");
            }
            uow.Commit();

            return Result.Ok(result);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}