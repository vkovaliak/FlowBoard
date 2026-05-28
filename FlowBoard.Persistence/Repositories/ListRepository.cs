using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using System.Data;

namespace FlowBoard.Persistence.Repositories;

public class ListRepository : BaseRepository<ListEntity, Guid>, IListRepository
{
    public ListRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal ListRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }
}