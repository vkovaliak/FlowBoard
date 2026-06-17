using Dapper;
using FlowBoard.Application.Abstractions;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class CardAssigneeRepository : ICardAssigneeRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public CardAssigneeRepository(ISqlConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
    }

    internal CardAssigneeRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AssignAsync(Guid cardId, Guid userId)
    {
        const string sql = """
            IF NOT EXISTS (SELECT 1 FROM CardAssignees 
                           WHERE CardId = @CardId AND UserId = @UserId)
            BEGIN
                INSERT INTO CardAssignees (CardId, UserId)
                VALUES (@CardId, @UserId);
            END
            """;

        await _connection.ExecuteAsync(sql,
            new { CardId = cardId, UserId = userId },
            _transaction);
    }

    public async Task UnassignAsync(Guid cardId, Guid userId)
    {
        const string sql = """
            DELETE FROM CardAssignees 
            WHERE CardId = @CardId AND UserId = @UserId;
            """;

        await _connection.ExecuteAsync(sql,
            new { CardId = cardId, UserId = userId },
            _transaction);
    }
}