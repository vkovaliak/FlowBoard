using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Search;

namespace FlowBoard.Persistence.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly IDbConnection _connection;

    public SearchRepository(ISqlConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
    }

    public async Task<List<SearchBoardDto>> SearchBoardsAsync(
        Guid userId, string query, int limit)
    {
        const string sql = """
            SELECT TOP (@Limit)
                b.Id,
                b.Name
            FROM Boards b
            INNER JOIN BoardMembers bm ON bm.BoardId = b.Id
            WHERE bm.UserId = @UserId
              AND b.Name LIKE @Pattern
            ORDER BY b.Name;
            """;

        var command = new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                Pattern = $"%{query}%", 
                Limit = limit
            });

        var result = await _connection.QueryAsync<SearchBoardDto>(
            command);
            
        return result.ToList();
    }

    public async Task<List<SearchCardDto>> SearchCardsAsync(
        Guid userId, string query, int limit)
    {
        const string sql = """
            SELECT TOP (@Limit)
                c.Id,
                c.Name,
                b.Id   AS BoardId,
                b.Name AS BoardName
            FROM Cards c
            INNER JOIN Lists l ON l.Id = c.ListId
            INNER JOIN Boards b ON b.Id = l.BoardId
            INNER JOIN BoardMembers bm ON bm.BoardId = b.Id
            WHERE bm.UserId = @UserId
              AND c.Name LIKE @Pattern
            ORDER BY c.Name;
            """;

        var command = new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                Pattern = $"%{query}%",
                Limit = limit
            });

        var result = await _connection.QueryAsync<SearchCardDto>(
            command);

        return result.ToList();
    }

    public async Task<List<UserSearchDto>> SearchUsersAsync(
        Guid currentUserId, string query, int limit)
    {
        const string sql = """
            SELECT TOP (@Limit)
                u.Id,
                u.EmailAddress,
                u.UserName,
                u.AvatarUrl
            FROM Users u
            WHERE (u.EmailAddress LIKE @Pattern OR u.UserName LIKE @Pattern)
            AND u.Id <> @CurrentUserId
            ORDER BY u.EmailAddress;
            """;

        var command = new CommandDefinition(
            sql,
            new
            {
                CurrentUserId = currentUserId,
                Pattern = $"%{query}%",
                Limit = limit
            });

        var result = await _connection.QueryAsync<UserSearchDto>(command);

        return result.ToList();
    }
}