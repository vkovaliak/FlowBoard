using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.UpdateList;

public class UpdateListCommandHandler : IRequestHandler<UpdateListCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateListCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateListCommand command, CancellationToken cancellationToken)
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

            var isMember = await uow.BoardRepository.IsMemberAsync(command.BoardId, currentUserId);
            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail(ErrorMessages.NoBoardAccess);
            }

            var list = await uow.ListRepository.GetByIdAsync(command.ListId);
            if (list is null)
            {
                return Result.Fail(ErrorMessages.ListNotFound);
            }

            list.Name = command.Name;
            list.UpdatedAt = DateTime.UtcNow;
            list.UpdatedBy = currentUserId;

            var result = await uow.ListRepository.UpdateAsync(list);
            uow.Commit();

            return Result.Ok(result);
        }
        catch
        {
            uow.Rollback();
            return Result.Fail("An error occurred while updating the list");
        }
    }
}