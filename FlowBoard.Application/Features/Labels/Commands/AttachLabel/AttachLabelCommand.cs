using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.AttachLabel;

public record AttachLabelCommand(
    Guid BoardId,
    Guid CardId,
    Guid LabelId)
    : IRequest<Result<bool>>;