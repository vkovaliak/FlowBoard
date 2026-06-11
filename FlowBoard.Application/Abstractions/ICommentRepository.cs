using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICommentRepository : IBaseRepository<Comment, Guid>
{
    
}