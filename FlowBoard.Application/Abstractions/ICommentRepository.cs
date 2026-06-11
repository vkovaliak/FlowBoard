using FlowBoard.Domain.DTOs.Comments;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICommentRepository : IBaseRepository<Comment, Guid>
{
    Task<IEnumerable<CommentDto>> GetCommentsByCardIdAsync(Guid cardId);
}