using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
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

        var result = boards.Select(b => new BoardDto(
            b.Id,
            b.Name,
            b.IsPublic,
            b.CreatedBy,
            b.CreatedAt
        )).ToList();

        return result;
    }
}