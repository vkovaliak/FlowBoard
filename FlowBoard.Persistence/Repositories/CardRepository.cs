using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class CardRepository : BaseRepository<Card, Guid>, ICardRepository
{
    public CardRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal CardRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public async Task<int> GetNextPositionAsync(Guid listId)
    {
        const string sql = """
            SELECT ISNULL (MAX(Position), -1) + 1 
            FROM Cards WHERE ListId = @ListId;
            """;

        return await _connection.QueryFirstOrDefaultAsync<int>(sql, 
            new { ListId = listId }, 
            _transaction);
    }

    public async Task ShiftPositionsAfterDeleteAsync(Guid listId, int deletedPosition)
    {
        const string sql = @"
            UPDATE Cards 
            SET Position = Position - 1 
            WHERE ListId = @ListId AND Position > @DeletedPosition;";

        await _connection.ExecuteAsync(sql, 
            new { ListId = listId, DeletedPosition = deletedPosition }, 
            _transaction);
    }

    public async Task ShiftPositionsOnMoveAsync(
        Guid fromListId, Guid toListId, int oldPosition, int newPosition)
    {
        const string sql = """
            IF @FromListId = @ToListId
            BEGIN
                IF @OldPosition > @NewPosition
                BEGIN
                    UPDATE Cards
                    SET Position = Position + 1
                    WHERE ListId = @FromListId 
                    AND Position >= @NewPosition 
                    AND Position < @OldPosition;
                END
                ELSE
                BEGIN
                    UPDATE Cards
                    SET Position = Position - 1
                    WHERE ListId = @FromListId 
                    AND Position > @OldPosition 
                    AND Position <= @NewPosition;
                END
            END
            ELSE
            BEGIN
                UPDATE Cards
                SET Position = Position + 1
                WHERE ListId = @ToListId 
                AND Position >= @NewPosition;

                UPDATE Cards
                SET Position = Position - 1
                WHERE ListId = @FromListId 
                AND Position > @OldPosition;
            END
            """;

        await _connection.ExecuteAsync(sql, new
        {
            FromListId = fromListId,
            ToListId = toListId,
            OldPosition = oldPosition,
            NewPosition = newPosition
        }, _transaction);
    }
}