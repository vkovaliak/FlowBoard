using FlowBoard.Domain.Entities;

namespace FlowBoard.Domain.DTOs.Boards;

public static class BoardMapping
{
    public static BoardDto ToDto(Board board)
    {
        return new BoardDto(
            board.Id,
            board.Name,
            board.IsPublic,
            board.CreatedBy,
            board.CreatedAt
        );
    }

    public static Board FromDto(BoardDto dto)
    {
        return new Board
        {
            Id = dto.Id,
            Name = dto.Name,
            IsPublic = dto.IsPublic,
            CreatedBy = dto.CreatedBy,
            CreatedAt = dto.CreatedAt
        };
    }
}