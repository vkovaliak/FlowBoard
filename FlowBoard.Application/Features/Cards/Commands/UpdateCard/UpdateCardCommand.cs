using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCard;

public record UpdateCardCommand(
    Guid BoardId,
    Guid ListId,
    Guid CardId,
    string Name,
    string? Description) 
    : IRequest<Result<bool>>;