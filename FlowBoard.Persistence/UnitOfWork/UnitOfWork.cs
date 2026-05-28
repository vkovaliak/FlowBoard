using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Repositories;

namespace FlowBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    private IBoardRepository? _boards;
    public IBoardRepository BoardRepository 
        => _boards ??= new BoardRepository(_connection, _transaction);

    private IListRepository? _lists;
    public IListRepository ListRepository
        => _lists ??= new ListRepository(_connection, _transaction);

    private IUserRepository? _users;
    public IUserRepository UserRepository 
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