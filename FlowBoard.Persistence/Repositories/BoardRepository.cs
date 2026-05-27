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
}