using System.Data;

namespace FlowBoard.Application.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}