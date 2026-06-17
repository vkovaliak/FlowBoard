using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.CreateLabel;

public class CreateLabelCommandHandler
    : IRequestHandler<CreateLabelCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public CreateLabelCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(
        CreateLabelCommand command, CancellationToken cancellationToken)
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

            var label = new Label
            {
                Id = Guid.NewGuid(),
                BoardId = command.BoardId,
                Name = command.Name,
                Color = command.Color,
                CreatedBy = currentUserId
            };

            await uow.LabelRepository.CreateAsync(label);

            uow.Commit();
            return Result.Ok(label.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}