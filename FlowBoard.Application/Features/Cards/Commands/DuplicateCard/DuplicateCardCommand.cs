using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.DuplicateCard;

public record DuplicateCardCommand(
    Guid BoardId,
    Guid CardId)
    : IRequest<Result<Guid>>;