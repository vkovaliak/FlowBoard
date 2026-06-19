using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCardDescription;

public record UpdateCardDescriptionCommand(
    Guid BoardId,
    Guid CardId,
    string? Description)
    : IRequest<Result<bool>>;