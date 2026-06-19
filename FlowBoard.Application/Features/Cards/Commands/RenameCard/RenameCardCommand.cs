using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.RenameCard;

public record RenameCardCommand(
    Guid BoardId,
    Guid CardId,
    string Name)
    : IRequest<Result<bool>>;