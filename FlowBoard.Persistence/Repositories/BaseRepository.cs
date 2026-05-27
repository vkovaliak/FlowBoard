using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    private readonly ISqlConnectionFactory? _connectionFactory;

    protected readonly IDbConnection? _activeConnection;
    protected readonly IDbTransaction? _activeTransaction;

    public BaseRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    internal BaseRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _activeConnection = connection;
        _activeTransaction = transaction;
    }

    private IDbConnection GetConnection() => _activeConnection ?? _connectionFactory!.CreateConnection();

    public async Task CreateAsync(TEntity entity)
    {
        if (_activeConnection != null)
        {
            await _activeConnection.InsertAsync<TId, TEntity>(entity, _activeTransaction);
        }
        else
        {
            using var connection = GetConnection();
            await connection.InsertAsync<TId, TEntity>(entity);
        }
    }

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        if (_activeConnection != null)
        {
            return await _activeConnection.GetAsync<TEntity>(id, _activeTransaction);
        }
        using var connection = GetConnection();
        return await connection.GetAsync<TEntity>(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
         if (_activeConnection != null)
        {
            return await _activeConnection.GetListAsync<TEntity>(_activeTransaction);
        }
        using var connection = GetConnection();
        return await connection.GetListAsync<TEntity>();
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        if (_activeConnection != null)
        {
            var result = await _activeConnection.UpdateAsync(entity, _activeTransaction);
            return result > 0;
        }
        else
        {
            using var connection = GetConnection();
            var result = await connection.UpdateAsync(entity);
            return result > 0;
        }
        
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        if (_activeConnection != null)
        {
            var result = await _activeConnection.DeleteAsync(entity, _activeTransaction);
            return result > 0;
        }
        else
        {
            using var connection = GetConnection();
            var result = await connection.DeleteAsync(entity);
            return result > 0;
        }
    }
}