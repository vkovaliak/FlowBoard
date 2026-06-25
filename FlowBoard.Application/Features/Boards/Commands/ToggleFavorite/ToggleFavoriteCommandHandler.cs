using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ToggleFavorite;

public class ToggleFavoriteCommandHandler
    : IRequestHandler<ToggleFavoriteCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public ToggleFavoriteCommandHandler(
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        ToggleFavoriteCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        using var uow = _uowFactory.Create();
        try
        {
            var success = await uow.BoardRepository.ToggleFavoriteAsync(
                command.BoardId, currentUserId);

            if (!success)
            {
                return Result.Fail("Board not found or you are not a member.");
            }

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