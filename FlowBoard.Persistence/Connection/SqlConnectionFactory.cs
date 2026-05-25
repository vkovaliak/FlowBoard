using System.Data;
using FlowBoard.Application.Abstractions;
using Microsoft.Data.SqlClient;

namespace FlowBoard.Connection.Persistence;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}