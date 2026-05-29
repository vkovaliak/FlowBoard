using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;

public class GetBoardDetailsQueryHandler : IRequestHandler<GetBoardDetailsQuery, BoardDetailsDto?>
{
    private readonly IBoardRepository _boardRepository;

    public GetBoardDetailsQueryHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardDetailsDto?> Handle(GetBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _boardRepository.GetDetailsAsync(request.BoardId);
    }
}