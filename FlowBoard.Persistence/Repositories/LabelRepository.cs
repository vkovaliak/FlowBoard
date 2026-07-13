using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Labels;
using FlowBoard.Domain.Entities;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class LabelRepository : BaseRepository<Label, Guid>, ILabelRepository
{
    public LabelRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    internal LabelRepository(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction) { }

    public async Task<IEnumerable<LabelDto>> GetByBoardIdAsync(Guid boardId)
    {
        const string sql = """
            SELECT Id, Name, Color
            FROM Labels
            WHERE BoardId = @BoardId
            ORDER BY Name;
            """;

        return await _connection.QueryAsync<LabelDto>(
            sql, 
            new { BoardId = boardId }, 
            _transaction);
    }

    public async Task RemoveByLabelIdAsync(Guid labelId)
    {
        const string sql = """
            DELETE FROM CardLabels 
            WHERE LabelId = @LabelId
            """;
            
        await _connection.ExecuteAsync(
            sql, new { LabelId = labelId }, _transaction);
    }
}