using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Labels.Commands.CreateLabel;

public record CreateLabelCommand(
    Guid BoardId,
    string Name,
    string Color)
    : IRequest<Result<Guid>>;