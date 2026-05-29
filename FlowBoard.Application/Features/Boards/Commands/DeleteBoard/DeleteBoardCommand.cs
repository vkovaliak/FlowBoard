using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(
    Guid BoardId,
    Guid CurrentUserId) 
    : IRequest<bool>;