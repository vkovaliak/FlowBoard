using FlowBoard.Application.Abstractions;

namespace FlowBoard.Persistence.UnitOfWork;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UnitOfWorkFactory(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IUnitOfWork> CreateAsync()
    {
        var connection = _connectionFactory.CreateConnection();
        return await Task.FromResult(new UnitOfWork(connection));
    }
}