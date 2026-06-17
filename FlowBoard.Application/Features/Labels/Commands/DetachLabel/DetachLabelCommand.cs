using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.DetachLabel;

public record DetachLabelCommand(
    Guid BoardId,
    Guid CardId,
    Guid LabelId)
    : IRequest<Result<bool>>;