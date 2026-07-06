using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.UpdateBoard;

public record UpdateBoardCommand(
    Guid BoardId,
    string Name, 
    bool IsPublic,
    string? Background) 
    : IRequest<Result<Guid>>;