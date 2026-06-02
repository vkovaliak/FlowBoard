using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    internal UserRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT * FROM Users 
            WHERE EmailAddress = @Email
            """;

        return await _connection.QueryFirstOrDefaultAsync<User>(
            sql, 
            new { Email = email }, 
            _transaction);
    }
}