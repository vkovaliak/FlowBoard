using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Comments;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class CommentRepository : BaseRepository<Comment, Guid>, ICommentRepository
{
    public CommentRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal CommentRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }
    
    public async Task<IEnumerable<CommentDto>> GetCommentsByCardIdAsync(Guid cardId)
    {
        string sql = @"
            SELECT 
                c.Id, 
                c.CardId, 
                c.Message, 
                c.CreatedAt, 
                c.CreatedBy,
                u.EmailAddress AS Email
            FROM Comments c
            INNER JOIN Users u ON c.CreatedBy = u.Id
            WHERE c.CardId = @CardId
            ORDER BY c.CreatedAt DESC";

        return await _connection.QueryAsync<CommentDto>(sql, new { 
            CardId = cardId }, 
            _transaction);
    }
}