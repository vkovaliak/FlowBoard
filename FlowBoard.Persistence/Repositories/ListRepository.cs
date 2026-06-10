using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class ListRepository : BaseRepository<ListEntity, Guid>, IListRepository
{
    public ListRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal ListRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public async Task<int> GetNextPositionAsync(Guid boardId)
    {
        const string sql = """
            SELECT ISNULL (MAX(Position), -1) + 1 
            FROM Lists WHERE BoardId = @BoardId;
            """;
            
        return await _connection.QueryFirstOrDefaultAsync<int>(sql, 
            new { BoardId = boardId }, 
            _transaction);
    }

    public async Task ShiftPositionsAfterDeleteAsync(Guid boardId, int deletedPosition)
    {
        const string sql = """
            UPDATE Lists 
            SET Position = Position - 1 
            WHERE BoardId = @BoardId AND Position > @DeletedPosition;
            """;

        await _connection.ExecuteAsync(sql, 
            new { BoardId = boardId, DeletedPosition = deletedPosition }, 
            _transaction);
    }
}