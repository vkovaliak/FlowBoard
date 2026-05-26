using FlowBoard.Application.Abstractions;
using Dapper;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(User user)
    {
        const string sql = """
            INSERT INTO Users
            (
                Id,
                EmailAddress,
                PasswordHash
            )
            VALUES
            (
                @Id,
                @EmailAddress,
                @PasswordHash
            )
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, user);
    }
}