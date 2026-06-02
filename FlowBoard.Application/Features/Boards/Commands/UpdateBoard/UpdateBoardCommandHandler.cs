using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.UpdateBoard;

public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand, bool>
{
    private readonly IBoardRepository _boardRepository;

    public UpdateBoardCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<bool> Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.BoardId);
        if (board == null
            || board.CreatedBy != request.CurrentUserId)
        {
            return false;
        }

        board.Name = request.Name;
        board.IsPublic = request.IsPublic;

        return await _boardRepository.UpdateAsync(board);
    }
}