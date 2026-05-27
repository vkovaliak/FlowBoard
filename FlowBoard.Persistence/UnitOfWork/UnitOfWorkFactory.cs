using FlowBoard.Application.Abstractions;

namespace FlowBoard.Persistence.UnitOfWork;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UnitOfWorkFactory(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IUnitOfWork Create()
    {
        return new UnitOfWork(_connectionFactory);
    }
}