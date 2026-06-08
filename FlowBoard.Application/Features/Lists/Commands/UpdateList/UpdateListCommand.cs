using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Lists.Commands.UpdateList;

public record UpdateListCommand(
    Guid BoardId,
    Guid ListId,
    string Name) : IRequest<Result<bool>>;