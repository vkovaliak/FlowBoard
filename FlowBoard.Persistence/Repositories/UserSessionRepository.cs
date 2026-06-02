using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class UserSessionRepository : BaseRepository<UserSession, Guid>, IUserSessionRepository
{
    public UserSessionRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal UserSessionRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public async Task<UserSession?> GetByTokenAsync(string token)
    {
        const string sql = """
            SELECT * FROM UserSessions 
            WHERE Token = @Token;
            """;
        
        return await _connection.QuerySingleOrDefaultAsync<UserSession>(
            sql, 
            new { Token = token }, 
            _transaction
        );
    }
}