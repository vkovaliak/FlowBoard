using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.DeleteCard;

public record DeleteCardCommand(
    Guid CardId,
    Guid ListId,
    Guid BoardId) 
    : IRequest<Result<bool>>;