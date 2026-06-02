using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IListRepository : IBaseRepository<ListEntity, Guid>
{
    Task<int> GetNextPositionAsync(Guid boardId);
}