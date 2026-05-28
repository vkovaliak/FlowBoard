namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IListRepository ListRepository { get; }
    IUserRepository UserRepository { get; }
    IBoardRepository BoardRepository { get; }
    
    void Commit();
    void Rollback();
}