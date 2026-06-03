using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.DeleteList;

public record DeleteListCommand(
    Guid ListId,
    Guid BoardId) 
    : IRequest<Result<bool>>;