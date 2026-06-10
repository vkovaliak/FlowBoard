using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IListRepository : IBaseRepository<ListEntity, Guid>
{
    Task<int> GetNextPositionAsync(Guid boardId);
    Task ShiftPositionsAfterDeleteAsync(Guid boardId, int deletedPosition);
    Task ShiftPositionsOnMoveAsync(Guid boardId, int oldPosition, int newPosition);
}