using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;

public class GetBoardDetailsQueryHandler : IRequestHandler<GetBoardDetailsQuery, Result<BoardDetailsDto?>>
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardDetailsQueryHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<Result<BoardDetailsDto?>> Handle(GetBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _boardRepository.GetDetailsAsync(request.BoardId);
        return Result.Ok(result);
    }
}