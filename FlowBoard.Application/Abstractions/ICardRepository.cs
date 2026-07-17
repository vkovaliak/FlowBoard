using FlowBoard.Domain.DTOs.Cards;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICardRepository : IBaseRepository<Card, Guid>
{
    Task<int> GetNextPositionAsync(Guid listId);
    Task ShiftPositionsAfterDeleteAsync(Guid listId, int deletedPosition);
    Task ShiftPositionsOnMoveAsync(Guid fromListId, Guid toListId, int oldPosition, int newPosition);
    Task ToggleCompletionAsync(Guid cardId);
    Task<IEnumerable<MyCardDto>> GetMyTasksAsync(Guid userId);
    Task CopyLabelsAsync(Guid sourceCardId, Guid targetCardId);
    Task CopyAssigneesAsync(Guid sourceCardId, Guid targetCardId);
    Task CopyChecklistItemsAsync(Guid sourceCardId, Guid targetCardId, Guid createdBy);
}