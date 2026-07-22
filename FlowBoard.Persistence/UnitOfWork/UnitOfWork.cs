using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Repositories;

namespace FlowBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    private IActivityRepository? _activity;
    public IActivityRepository ActivityRepository
        => _activity ??= new ActivityRepository(_connection, _transaction);

    private IBoardRepository? _boards;
    public IBoardRepository BoardRepository 
        => _boards ??= new BoardRepository(_connection, _transaction);
    
    private ICardRepository? _cards;
    public ICardRepository CardRepository
        => _cards ??= new CardRepository(_connection, _transaction);

    private ICardAssigneeRepository? _cardAssignees;
    public ICardAssigneeRepository CardAssigneeRepository
        => _cardAssignees ??= new CardAssigneeRepository(_connection, _transaction);
    
    private ICardLabelRepository? _cardLabels;
    public ICardLabelRepository CardLabelRepository
        => _cardLabels ??= new CardLabelRepository(_connection, _transaction);

    private IChecklistItemRepository? _checklistItems;
    public IChecklistItemRepository ChecklistItemRepository
        => _checklistItems ??= new ChecklistItemRepository(_connection, _transaction);
        
    private ICommentRepository? _comment;
    public ICommentRepository CommentRepository
        => _comment ??= new CommentRepository(_connection, _transaction);

    private ILabelRepository? _labels;
    public ILabelRepository LabelRepository
        => _labels ??= new LabelRepository(_connection, _transaction);

    private IListRepository? _lists;
    public IListRepository ListRepository
        => _lists ??= new ListRepository(_connection, _transaction);

    private IUserRepository? _users;
    public IUserRepository UserRepository 
        => _users ??= new UserRepository(_connection, _transaction);
    
    private IUserSessionRepository? _userSessions;
    public IUserSessionRepository UserSessionRepository
        => _userSessions ??= new UserSessionRepository(_connection, _transaction);


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