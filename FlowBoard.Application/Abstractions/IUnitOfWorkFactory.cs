namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}