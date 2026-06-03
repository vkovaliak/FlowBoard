using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public CreateCardCommandHandler(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<Result<Guid>> Handle(CreateCardCommand command, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var board = await uow.BoardRepository.GetByIdAsync(command.BoardId);
            if (board is null)
            {
                return Result.Fail("Board not found");
            }

            var isMember = await uow.BoardRepository.IsMemberAsync(command.BoardId, command.CurrentUserId);

            if (!isMember && board.CreatedBy != command.CurrentUserId)
            {
                return Result.Fail("You don't have access to this board");
            }

            var position = await uow.CardRepository.GetNextPositionAsync(command.ListId);

            var card = new Card
            {
                Id = Guid.NewGuid(),
                ListId = command.ListId,
                Name = command.Name,
                Description = command.Description,
                Position = position,
                CreatedBy = command.CurrentUserId,
            };

            await uow.CardRepository.CreateAsync(card);

            uow.Commit();
            return Result.Ok(card.Id);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}