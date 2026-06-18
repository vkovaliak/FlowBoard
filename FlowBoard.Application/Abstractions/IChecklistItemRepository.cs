using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IChecklistItemRepository 
    : IBaseRepository<ChecklistItem, Guid>
{
    Task<int> GetMaxPositionAsync(Guid cardId);
}