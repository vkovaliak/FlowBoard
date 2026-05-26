using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Configurations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FlowBoard.Persistence.Connection;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(IOptions<DatabaseOptions> databaseOptions)
    {
        _connectionString = databaseOptions.Value.ConnectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}