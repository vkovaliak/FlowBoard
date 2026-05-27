using System.Data;

namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    
    Task CommitAsync();
    Task RollbackAsync();
}