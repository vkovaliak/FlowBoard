using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class BaseRepository<TEntity, TId> 
    : IBaseRepository<TEntity, TId>, IDisposable 
    where TEntity : BaseEntity<TId>
{
    protected readonly IDbConnection _connection;
    protected readonly IDbTransaction? _transaction;

    public BaseRepository(ISqlConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
    }

    internal BaseRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public Task CreateAsync(TEntity entity)
    {
        return _connection.InsertAsync<TId, TEntity>(entity, _transaction);
    }

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await _connection.GetAsync<TEntity>(id, _transaction);
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return _connection.GetListAsync<TEntity>(_transaction);
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        var result = await _connection.UpdateAsync(entity, _transaction);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        var result = await _connection.DeleteAsync(entity, _transaction);
        return result > 0;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}