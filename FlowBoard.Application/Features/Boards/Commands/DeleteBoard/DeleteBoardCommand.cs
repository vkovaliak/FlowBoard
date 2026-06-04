using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(
    Guid BoardId) 
    : IRequest<Result<bool>>;