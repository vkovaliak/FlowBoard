using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICardRepository : IBaseRepository<Card, Guid>
{
    Task<int> GetNextPositionAsync(Guid listId);
}