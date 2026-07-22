using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Activities;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class ActivityRepository : BaseRepository<Activity, Guid>, IActivityRepository
{
    public ActivityRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }
    
    internal ActivityRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public async Task<IEnumerable<ActivityDto>> GetByCardIdAsync(Guid cardId)
    {
        const string sql = """
            SELECT 
                a.Id,
                a.CardId,
                a.UserId,
                u.UserName,
                u.AvatarUrl,
                a.ActionType,
                a.Description,
                a.CreatedAt
            FROM Activities a
            JOIN Users u ON u.Id = a.UserId
            WHERE a.CardId = @CardId
            ORDER BY a.CreatedAt DESC;
            """;

        return await _connection.QueryAsync<ActivityDto>(
            sql, new { CardId = cardId }, _transaction);
    }
}