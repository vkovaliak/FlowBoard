using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.CreateList;

public record CreateListCommand(
    Guid BoardId,
    string Name,
    Guid CurrentUserId)
    : IRequest<Guid>;