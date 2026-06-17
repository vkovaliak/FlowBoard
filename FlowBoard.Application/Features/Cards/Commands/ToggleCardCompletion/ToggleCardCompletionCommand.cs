using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.ToggleCardCompletion;

public record ToggleCardCompletionCommand(
    Guid BoardId,
    Guid CardId)
    : IRequest<Result<bool>>;