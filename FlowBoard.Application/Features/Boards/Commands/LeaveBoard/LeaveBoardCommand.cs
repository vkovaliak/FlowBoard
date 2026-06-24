using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.LeaveBoard;

public record LeaveBoardCommand(
    Guid BoardId)
    : IRequest<Result<bool>>;