namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBoardRepository Boards { get; }
    
    void Commit();
    void Rollback();
}