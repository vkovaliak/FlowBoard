using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class CommentAtachmentsRepository : BaseRepository<CommentAttachment, Guid>, ICommentAttachmentRepository
{
    public CommentAtachmentsRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    internal CommentAtachmentsRepository(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction) { }
}