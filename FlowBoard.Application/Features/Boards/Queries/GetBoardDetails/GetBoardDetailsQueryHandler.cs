using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;

public class GetBoardDetailsQueryHandler : IRequestHandler<GetBoardDetailsQuery, Result<BoardDetailsDto?>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetBoardDetailsQueryHandler(IBoardRepository boardRepository, ICurrentUserService currentUserService)
    {
        _boardRepository = boardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BoardDetailsDto?>> Handle(GetBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        var result = await _boardRepository.GetDetailsAsync(request.BoardId, currentUserId);
        return Result.Ok(result);
    }
}