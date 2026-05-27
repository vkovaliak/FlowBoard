namespace FlowBoard.Application.Abstractions;

public interface IBaseRepository<TEntity>
{
    Task CreateAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);

}