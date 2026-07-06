using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.CreateBoard;

public record CreateBoardCommand(
    string Name, 
    bool IsPublic,
    string? Background) 
    : IRequest<Result<Guid>>;