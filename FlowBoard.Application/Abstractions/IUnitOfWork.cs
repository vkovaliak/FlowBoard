namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IListRepository Lists { get; }
    IUserRepository Users { get; }
    IBoardRepository Boards { get; }
    
    void Commit();
    void Rollback();
}