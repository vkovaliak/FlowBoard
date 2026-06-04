using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Mappings;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetMyBoards;

public class GetMyBoardsQueryHandler : IRequestHandler<GetMyBoardsQuery, Result<IEnumerable<BoardDto>>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyBoardsQueryHandler(IBoardRepository boardRepository, ICurrentUserService currentUserService)
    {
        _boardRepository = boardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<BoardDto>>> Handle(GetMyBoardsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        var boards = await _boardRepository.GetByUserIdAsync(currentUserId);
        var result =  boards.Select(BoardMapping.ToDto).ToList().AsEnumerable();

        return Result.Ok(result);
    }
}