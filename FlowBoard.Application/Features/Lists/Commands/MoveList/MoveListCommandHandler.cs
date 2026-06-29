using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.MoveList;

public class MoveListCommandHandler : IRequestHandler<MoveListCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public MoveListCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(MoveListCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var list = await uow.ListRepository.GetByIdAsync(request.ListId);
        
            if (list == null) 
            {
                return Result.Fail(ErrorMessages.ListNotFound);
            }

            if (list.BoardId != request.BoardId)
            {
                return Result.Fail(ErrorMessages.BoardNotFound);
            }

            int oldPosition = list.Position;
            int newPosition = request.NewPosition;

            if (oldPosition == newPosition)
            {
                return true;
            } 

            await uow.ListRepository.ShiftPositionsOnMoveAsync(
                request.BoardId, oldPosition, newPosition);

            list.Position = newPosition;
            list.UpdatedAt = DateTime.UtcNow;
            list.UpdatedBy = currentUserId;

            await uow.ListRepository.UpdateAsync(list);

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