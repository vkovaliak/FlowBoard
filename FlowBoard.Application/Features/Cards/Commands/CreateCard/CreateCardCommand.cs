using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.CreateCard;

public record CreateCardCommand(
    Guid ListId,
    Guid BoardId,
    string Name,
    string? Description,
    Guid CurrentUserId)
    : IRequest<Result<Guid>>;