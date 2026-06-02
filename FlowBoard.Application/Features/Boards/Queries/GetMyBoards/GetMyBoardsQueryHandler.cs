using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Mappings;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetMyBoards;

public class GetMyBoardsQueryHandler : IRequestHandler<GetMyBoardsQuery, IEnumerable<BoardDto>>
{
    private readonly IBoardRepository _boardRepository;

    public GetMyBoardsQueryHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetMyBoardsQuery request, CancellationToken cancellationToken)
    {
        var boards = await _boardRepository.GetByUserIdAsync(request.CurrentUserId);
        return boards.Select(BoardMapping.ToDto).ToList();
    }
}