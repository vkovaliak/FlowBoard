using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICardRepository : IBaseRepository<Card, Guid>
{
    Task<int> GetNextPositionAsync(Guid listId);
    Task ShiftPositionsAfterDeleteAsync(Guid listId, int deletedPosition);
    Task ShiftPositionsOnMoveAsync(Guid fromListId, Guid toListId, int oldPosition, int newPosition);
}