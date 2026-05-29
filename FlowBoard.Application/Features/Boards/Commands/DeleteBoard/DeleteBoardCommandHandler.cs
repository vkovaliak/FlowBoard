using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand, bool>
{
    private readonly IBoardRepository _boardRepository;

    public DeleteBoardCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<bool> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.BoardId);
        if (board == null)
        {
            return false;
        }
        if (board.CreatedBy != request.CurrentUserId)
        {
            return false;
        }

        var result = await _boardRepository.DeleteAsync(board);
        return result;

    }
}