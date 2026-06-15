using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ICardAttachmentRepository : IBaseRepository<CardAttachment, Guid>
{
}