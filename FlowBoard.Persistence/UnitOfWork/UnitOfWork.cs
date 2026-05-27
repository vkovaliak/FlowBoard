using System.Data;
using FlowBoard.Application.Abstractions;

namespace FlowBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    public readonly IDbTransaction _transaction;

    public IDbConnection Connection => _connection;
    public IDbTransaction Transaction => _transaction;

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;

        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        _transaction = _connection.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        _transaction.Commit();
        await Task.CompletedTask;
    }

    public async Task RollbackAsync()
    {
        _transaction.Rollback();
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _connection.Dispose();
    }
}