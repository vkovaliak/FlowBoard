using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.MoveCard;

public record MoveCardCommand(
    Guid BoardId,
    Guid CardId,
    Guid NewListId,
    int NewPosition) 
    : IRequest<Result<bool>>;