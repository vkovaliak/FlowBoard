using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IBaseRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    Task CreateAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);

}