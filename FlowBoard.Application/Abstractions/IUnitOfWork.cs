namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IListRepository ListRepository { get; }
    IUserRepository UserRepository { get; }
    IUserSessionRepository UserSessionRepository { get; }
    IBoardRepository BoardRepository { get; }
    ICardRepository CardRepository { get; }
    ICommentRepository CommentRepository { get; }
    ICardAssigneeRepository CardAssigneeRepository { get; }
    
    void Commit();
    void Rollback();
}