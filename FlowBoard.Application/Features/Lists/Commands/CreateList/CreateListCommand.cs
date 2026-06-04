using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.CreateList;

public record CreateListCommand(
    Guid BoardId,
    string Name)
    : IRequest<Result<Guid>>;