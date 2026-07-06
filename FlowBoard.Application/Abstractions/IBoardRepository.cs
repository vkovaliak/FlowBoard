using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Application.Abstractions;

public interface IBoardRepository : IBaseRepository<Board, Guid>
{
    Task AddMemberAsync(Guid boardId, Guid userId, BoardRole role);
    Task<IEnumerable<BoardDto>> GetByUserIdAsync(Guid userId, ArchiveStatus status);
    Task<BoardDetailsDto?> GetDetailsAsync(Guid boardId, Guid userId);
    Task<bool> IsMemberAsync(Guid boardId, Guid userId);
    Task<BoardRole?> GetUserRoleAsync(Guid boardId, Guid userId);
    Task<bool> RemoveMemberAsync(Guid boardId, Guid userId);
    Task<bool> ToggleFavoriteAsync(Guid boardId, Guid userId);
    Task<IEnumerable<BoardArchiveDto>> GetByArchiveStatusAsync(ArchiveStatus status);
    Task UpdateArchiveStatusAsync(Guid boardId, ArchiveStatus status);
    Task<BoardArchiveDto?> GetForArchiveAsync(Guid boardId);
    Task DeleteBoardContentAsync(Guid boardId);
    Task<int> GetOwnerCountAsync(Guid boardId);
    Task<bool> UpdateMemberRoleAsync(Guid boardId, Guid userId, BoardRole role);
    Task TransferOwnershipAsync(Guid boardId, Guid currentOwnerId, Guid newOwnerId);
    Task<int> GetOwnedBoardsCountAsync(Guid userId);
    Task<int> GetMembersCountAsync(Guid boardId);
}