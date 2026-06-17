using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.UpdateLabel;

public class UpdateLabelCommandHandler
    : IRequestHandler<UpdateLabelCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateLabelCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        UpdateLabelCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(command.BoardId);
            if (board is null)
            {
                return Result.Fail("Board not found");
            }

            var isMember = await uow.BoardRepository.IsMemberAsync(
                command.BoardId, currentUserId);
            if (!isMember && board.CreatedBy != currentUserId)
            {
                return Result.Fail("You don't have access to this board");
            }

            var label = await uow.LabelRepository.GetByIdAsync(command.LabelId);
            if (label is null || label.BoardId != command.BoardId)
            {
                return Result.Fail("Label not found on this board");
            }

            label.Name = command.Name;
            label.Color = command.Color;
            label.UpdatedAt = DateTime.UtcNow;
            label.UpdatedBy = currentUserId;

            var result = await uow.LabelRepository.UpdateAsync(label);

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