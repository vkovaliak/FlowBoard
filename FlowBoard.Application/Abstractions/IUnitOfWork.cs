namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    
    void Commit();
    void Rollback();
}