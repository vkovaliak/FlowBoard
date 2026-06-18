using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class ChecklistItemRepository 
    : BaseRepository<ChecklistItem, Guid>, IChecklistItemRepository
{
    public ChecklistItemRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    internal ChecklistItemRepository(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction) { }

    public async Task<int> GetMaxPositionAsync(Guid cardId)
    {
        const string sql = """
            SELECT ISNULL(MAX(Position), -1)
            FROM ChecklistItems
            WHERE CardId = @CardId;
            """;

        return await _connection.ExecuteScalarAsync<int>(
            sql, 
            new { CardId = cardId }, 
            _transaction);
    }
}