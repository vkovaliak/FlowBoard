using Dapper;
using FlowBoard.Application.Abstractions;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class CardLabelRepository : ICardLabelRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public CardLabelRepository(ISqlConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
    }

    internal CardLabelRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AttachAsync(Guid cardId, Guid labelId)
    {
        const string sql = """
            IF NOT EXISTS (SELECT 1 FROM CardLabels 
                           WHERE CardId = @CardId AND LabelId = @LabelId)
            BEGIN
                INSERT INTO CardLabels (CardId, LabelId)
                VALUES (@CardId, @LabelId);
            END
            """;

        await _connection.ExecuteAsync(
            sql,
            new { CardId = cardId, LabelId = labelId }, 
            _transaction);
    }

    public async Task DetachAsync(Guid cardId, Guid labelId)
    {
        const string sql = """
            DELETE FROM CardLabels 
            WHERE CardId = @CardId AND LabelId = @LabelId;
            """;

        await _connection.ExecuteAsync(sql,
            new { CardId = cardId, LabelId = labelId }, 
            _transaction);
    }
}