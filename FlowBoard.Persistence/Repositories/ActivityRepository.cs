using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Activities;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class ActivityRepository : BaseRepository<Activity, Guid>, IActivityRepository
{
    public ActivityRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    public async Task<IEnumerable<ActivityDto>> GetByBoardIdAsync(
        Guid boardId, int limit = 50)
    {
        const string sql = """
            SELECT TOP (@Limit)
                a.Id,
                a.UserId,
                u.UserName,
                u.AvatarUrl,
                a.ActionType,
                a.Description,
                a.CreatedAt
            FROM Activities a
            JOIN Users u ON u.Id = a.UserId
            WHERE a.BoardId = @BoardId
            ORDER BY a.CreatedAt DESC;
            """;

        return await _connection.QueryAsync<ActivityDto>(
            sql, new { BoardId = boardId, Limit = limit }, 
            _transaction);
    }
}