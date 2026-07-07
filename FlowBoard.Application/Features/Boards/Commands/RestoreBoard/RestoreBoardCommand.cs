using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.RestoreBoard;
public record RestoreBoardCommand(
    Guid BoardId) 
    : IRequest<Result<bool>>;