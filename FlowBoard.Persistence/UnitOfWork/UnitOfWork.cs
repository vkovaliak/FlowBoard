using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Repositories;

namespace FlowBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    private IBoardRepository? _boards;
    public IBoardRepository Boards 
        => _boards ??= new BoardRepository(_connection, _transaction);

    private IUserRepository? _users;
    public IUserRepository Users 
        => _users ??= new UserRepository(_connection, _transaction);


    public UnitOfWork(ISqlConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();

        _connection.Open();

        _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
        _transaction.Commit();
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _connection.Dispose();
    }
}