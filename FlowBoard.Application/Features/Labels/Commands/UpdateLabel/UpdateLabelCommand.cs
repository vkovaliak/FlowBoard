using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.UpdateLabel;

public record UpdateLabelCommand(
    Guid BoardId,
    Guid LabelId,
    string Name,
    string Color)
    : IRequest<Result<bool>>;