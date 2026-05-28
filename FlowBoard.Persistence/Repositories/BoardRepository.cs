using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class BoardRepository : BaseRepository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal BoardRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public Task AddMemberAsync(Guid boardId, Guid userId)
    {
        const string sql = @"
            INSERT INTO BoardMembers (BoardId, UserId) 
            VALUES (@BoardId, @UserId);";

        return _connection.ExecuteAsync(sql, new { BoardId = boardId, UserId = userId }, _transaction);
    }

    public async Task<IEnumerable<Board>> GetByUserIdAsync(Guid userId)
    {
        const string sql = @"
            SELECT DISTINCT 
                b.Id, 
                b.Name, 
                b.IsPublic, 
                b.CreatedBy, 
                b.CreatedAt
            FROM Boards b
            LEFT JOIN BoardMembers bm ON b.Id = bm.BoardId
            WHERE b.CreatedBy = @UserId 
               OR bm.UserId = @UserId
            ORDER BY b.CreatedAt DESC";

        return await _connection.QueryAsync<Board>(sql, new { UserId = userId });
    }
}