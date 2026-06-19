using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Attachments;
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
        const string sql = """
            SELECT 
                c.Id, 
                c.CardId, 
                c.Message, 
                c.CreatedAt, 
                c.CreatedBy,
                u.EmailAddress AS Email,
                u.UserName,
                u.AvatarUrl,

                a.Id,
                a.FileName,
                a.BlobUrl
            FROM Comments c
            INNER JOIN Users u ON c.CreatedBy = u.Id
            LEFT JOIN CommentAttachments a ON c.Id = a.CommentId
            WHERE c.CardId = @CardId
            ORDER BY c.CreatedAt DESC;
            """;

        var commentDictionary = new Dictionary<Guid, CommentDto>();

        await _connection.QueryAsync<CommentDto, AttachmentResponseDto, CommentDto>(
            sql,
            (comment, attachment) =>
            {
                if (!commentDictionary.TryGetValue(comment.Id, out var commentEntry))
                {
                    commentEntry = comment;
                    commentEntry.Attachments = new List<AttachmentResponseDto>();
                    commentDictionary.Add(commentEntry.Id, commentEntry);
                }

                if (attachment is not null && attachment.Id != Guid.Empty)
                {
                    ((List<AttachmentResponseDto>)commentEntry.Attachments).Add(attachment);
                }

                return commentEntry;
            },
            new { CardId = cardId },
            _transaction,
            splitOn: "Id");

        return commentDictionary.Values;
    }
}