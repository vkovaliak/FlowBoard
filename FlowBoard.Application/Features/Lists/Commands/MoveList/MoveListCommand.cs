using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.MoveList;

public record MoveListCommand(
    Guid BoardId,
    Guid ListId,
    int NewPosition) 
    : IRequest<Result<bool>>;