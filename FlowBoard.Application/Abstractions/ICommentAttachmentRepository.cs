using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICommentAttachmentRepository : IBaseRepository<CommentAttachment, Guid>
{
}