using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.TransferOwnership;

public record TransferOwnershipCommand(
    Guid BoardId, Guid NewOwnerId)
    : IRequest<Result<bool>>;