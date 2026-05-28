using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.DTOs.Cards;
using FlowBoard.Domain.DTOs.Lists;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Persistence.Repositories;

public class BoardRepository : BaseRepository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal BoardRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public Task AddMemberAsync(Guid boardId, Guid userId)
    {
        const string sql = @"
            INSERT INTO BoardMembers (BoardId, UserId) 
            VALUES (@BoardId, @UserId);";

        return _connection.ExecuteAsync(sql, new { BoardId = boardId, UserId = userId }, _transaction);
    }

    public async Task<IEnumerable<Board>> GetByUserIdAsync(Guid userId)
    {
        const string sql = @"
            SELECT DISTINCT 
                b.Id, 
                b.Name, 
                b.IsPublic, 
                b.CreatedBy, 
                b.CreatedAt
            FROM Boards b
            LEFT JOIN BoardMembers bm ON b.Id = bm.BoardId
            WHERE b.CreatedBy = @UserId 
               OR bm.UserId = @UserId
            ORDER BY b.CreatedAt DESC";

        return await _connection.QueryAsync<Board>(sql, new { UserId = userId });
    }

    public async Task<BoardDetailsDto?> GetDetailsAsync(Guid boardId)
{
    const string sql = """
        SELECT
            b.Id,
            b.Name,
            b.IsPublic,
            b.CreatedBy,
            b.CreatedAt,

            l.Id,
            l.BoardId,
            l.Name,
            l.Position,

            c.Id,
            c.Name,
            c.Description,
            c.Position

        FROM Boards b
        LEFT JOIN Lists l ON l.BoardId = b.Id
        LEFT JOIN Cards c ON c.ListId = l.Id

        WHERE b.Id = @BoardId

        ORDER BY l.Position, c.Position;
        """;


    var boardDictionary = new Dictionary<Guid, BoardDetailsDto>();
    var listDictionary = new Dictionary<Guid, ListDto>();

    var result = await _connection.QueryAsync<
        BoardDetailsDto,
        ListDto,
        CardDto,
        BoardDetailsDto>(
        sql,
        (board, list, card) =>
        {
            if (!boardDictionary.TryGetValue(board.Id, out var boardEntry))
            {
                boardEntry = board;
                boardEntry.Lists = [];

                boardDictionary.Add(boardEntry.Id, boardEntry);
            }

            if (list is not null)
            {
                if (!listDictionary.TryGetValue(list.Id, out var listEntry))
                {
                    listEntry = list;
                    listEntry.Cards = [];

                    listDictionary.Add(listEntry.Id, listEntry);

                    boardEntry.Lists.Add(listEntry);
                }

                if (card is not null)
                {
                    listEntry.Cards.Add(card);
                }
            }

            return boardEntry;
        },
        new { BoardId = boardId },
        splitOn: "Id,Id");

    return boardDictionary.Values.FirstOrDefault();
}
}