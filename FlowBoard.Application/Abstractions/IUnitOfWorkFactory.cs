namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> CreateAsync();
}