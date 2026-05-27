using FlowBoard.Application.Abstractions;
using Dapper;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
}