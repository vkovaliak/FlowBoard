using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.DeleteList;

public record DeleteListCommand(
    Guid ListId,
    Guid BoardId,
    Guid CurrentUserId) 
    : IRequest<bool>;