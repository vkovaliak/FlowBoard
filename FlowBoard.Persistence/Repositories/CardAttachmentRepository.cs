using System.Data;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class CardAttachmentsRepository : BaseRepository<CardAttachment, Guid>, ICardAttachmentRepository
{
    public CardAttachmentsRepository(ISqlConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    internal CardAttachmentsRepository(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction) { }
}