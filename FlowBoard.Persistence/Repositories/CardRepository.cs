using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.CardAssignee;
using FlowBoard.Domain.DTOs.Cards;
using FlowBoard.Domain.DTOs.Labels;
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

    public Task ToggleCompletionAsync(Guid cardId)
    {
        const string sql = """
            UPDATE Cards SET IsCompleted = ~IsCompleted 
            WHERE Id = @CardId;
            """;
            
        return _connection.ExecuteAsync(
            sql, 
            new { CardId = cardId }, 
            _transaction);
    }

    public async Task<IEnumerable<MyCardDto>> GetMyTasksAsync(Guid userId)
    {
        const string sqlCards = """
            SELECT
                c.Id,
                c.ListId,
                li.BoardId,
                b.Name AS BoardName,
                c.Name,
                c.Description,
                c.DueDate,
                c.IsCompleted
            FROM Cards c
            JOIN CardAssignees ca ON ca.CardId = c.Id
            JOIN Lists li ON li.Id = c.ListId
            JOIN Boards b ON b.Id = li.BoardId
            WHERE ca.UserId = @UserId
            ORDER BY c.DueDate;
            """;

        var cards = (await _connection.QueryAsync<MyCardDto>(
            sqlCards, new { UserId = userId })).ToList();

        if (cards.Count == 0)
        {
            return cards;
        }

        var cardIds = cards.Select(c => c.Id).ToList();

        const string sqlLabels = """
            SELECT
                cl.CardId,
                l.Id,
                l.Name,
                l.Color
            FROM CardLabels cl
            JOIN Labels l ON l.Id = cl.LabelId
            WHERE cl.CardId IN @CardIds;
            """;

        var labelRows = await _connection.QueryAsync<(
            Guid CardId, Guid Id, string Name, string Color)>(
            sqlLabels, new { CardIds = cardIds });

        var labelsByCard = labelRows
            .GroupBy(r => r.CardId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => new LabelDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Color = r.Color
                }).ToList());

        const string sqlAssignees = """
            SELECT
                ca.CardId,
                ca.UserId,
                u.EmailAddress,
                u.UserName,
                u.AvatarUrl
            FROM CardAssignees ca
            JOIN Users u ON u.Id = ca.UserId
            WHERE ca.CardId IN @CardIds;
            """;

        var assigneeRows = await _connection.QueryAsync<(
            Guid CardId, Guid UserId, string EmailAddress, 
            string UserName, string? AvatarUrl)>(
            sqlAssignees, new { CardIds = cardIds });

        var assigneesByCard = assigneeRows
            .GroupBy(r => r.CardId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => new CardAssigneeDto(
                    r.UserId, r.EmailAddress, r.UserName, 
                    r.AvatarUrl)).ToList());

        foreach (var card in cards)
        {
            if (labelsByCard.TryGetValue(card.Id, out var labels))
            {
                card.Labels = labels;
            }
            if (assigneesByCard.TryGetValue(card.Id, out var assignees))
            {
                card.Assignees = assignees;
            }
        }

        return cards;
    }
}