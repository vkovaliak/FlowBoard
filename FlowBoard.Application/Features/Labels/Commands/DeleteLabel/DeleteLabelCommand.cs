using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.DeleteLabel;

public record DeleteLabelCommand(
    Guid BoardId,
    Guid LabelId)
    : IRequest<Result<bool>>;