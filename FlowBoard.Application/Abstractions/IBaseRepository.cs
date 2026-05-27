namespace FlowBoard.Application.Abstractions;

public interface IBaseRepository<TEntity, TId> where TEntity : class
{
    Task CreateAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);

}