using Dapper;
using FlowBoard.Application.Abstractions;

namespace FlowBoard.Persistence.Repositories;

public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public BaseRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(TEntity entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.InsertAsync<TId, TEntity>(entity);
    }

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.GetAsync<TEntity>(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.GetListAsync<TEntity>();
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.UpdateAsync(entity);

        return result > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var result = await connection.DeleteAsync(entity);

        return result > 0;
    }
}