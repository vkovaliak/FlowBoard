using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Authorization;
using FlowBoard.Domain.DTOs.Boards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;

public class GetBoardDetailsQueryHandler 
    : IRequestHandler<GetBoardDetailsQuery, Result<BoardDetailsDto>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetBoardDetailsQueryHandler(IBoardRepository boardRepository, ICurrentUserService currentUserService)
    {
        _boardRepository = boardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BoardDetailsDto>> Handle(
        GetBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        
        var access = await _boardRepository.GetBoardAccessAsync(
            request.BoardId, currentUserId);
        if (access is null)
        {
            return Result.Fail("Board not found.");
        }

        var effectiveRole = BoardPermissions.ResolveEffectiveRole(access);
        if (effectiveRole is null)
        {
            return Result.Fail("You do not have access to this board.");
        }
        
        var board = await _boardRepository.GetDetailsAsync(
            request.BoardId, currentUserId);
        if (board is null)
        {
            return Result.Fail("Board not found.");
        }

        board.UserRole = effectiveRole.Value;

        return Result.Ok(board);
    }
}