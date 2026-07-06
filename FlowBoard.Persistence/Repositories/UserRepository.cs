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

    public async Task<User?> GetByExternalIdAsync(string provider, string externalId)
    {
        const string sql = """
            SELECT * FROM Users
            WHERE ExternalProvider = @Provider
            AND ExternalId = @ExternalId
            """;

        return await _connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Provider = provider, ExternalId = externalId },
            _transaction);
    }

    public async Task<bool> UpdatePasswordAsync(Guid userId, string passwordHash)
    {
        const string sql = """
            UPDATE Users
            SET PasswordHash = @PasswordHash
            WHERE Id = @UserId
            """;

        var result = await _connection.ExecuteAsync(
            sql,
            new { UserId = userId, PasswordHash = passwordHash },
            _transaction);
        
         return result > 0;
    }

    public async Task<User?> GetByStripeCustomerIdAsync(string customerId)
    {
        const string sql = """
            SELECT * FROM Users
            WHERE StripeCustomerId = @CustomerId;
            """;

        return await _connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { CustomerId = customerId },
            _transaction);
    }
}