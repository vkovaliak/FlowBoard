using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Application.Abstractions;

public interface IBoardRepository : IBaseRepository<Board, Guid>
{
    Task AddMemberAsync(Guid boardId, Guid userId, BoardRole role);
    Task<IEnumerable<Board>> GetByUserIdAsync(Guid userId);
    Task<BoardDetailsDto?> GetDetailsAsync(Guid boardId, Guid userId);
    Task<bool> IsMemberAsync(Guid boardId, Guid userId);
    Task<BoardRole?> GetUserRoleAsync(Guid boardId, Guid userId);
    Task<bool> RemoveMemberAsync(Guid boardId, Guid userId);
}