using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IBoardRepository : IBaseRepository<Board, Guid>
{
    Task AddMemberAsync(Guid boardId, Guid userId);
    Task<IEnumerable<Board>> GetByUserIdAsync(Guid userId);
    Task<BoardDetailsDto?> GetDetailsAsync(Guid boardId);
}