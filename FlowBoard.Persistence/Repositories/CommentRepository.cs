using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class CommentRepository : BaseRepository<Comment, Guid>, ICommentRepository
{
    public CommentRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal CommentRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }
}