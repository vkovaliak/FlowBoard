using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.SetCardStartTime;

public record SetCardStartTimeCommand(
    Guid BoardId,
    Guid CardId,
    DateTime? StartTime)
    : IRequest<Result<bool>>;