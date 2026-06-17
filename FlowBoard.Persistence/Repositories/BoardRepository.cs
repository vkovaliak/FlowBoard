using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Attachments;
using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.DTOs.CardAssignee;
using FlowBoard.Domain.DTOs.Cards;
using FlowBoard.Domain.DTOs.Lists;
using FlowBoard.Domain.DTOs.Users;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Persistence.Repositories;

public class BoardRepository : BaseRepository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ISqlConnectionFactory connectionFactory) 
        : base(connectionFactory) { }

    internal BoardRepository(IDbConnection connection, IDbTransaction transaction) 
        : base(connection, transaction) { }

    public Task AddMemberAsync(Guid boardId, Guid userId, BoardRole role)
    {
        const string sql = @"
            INSERT INTO BoardMembers (BoardId, UserId, Role) 
            VALUES (@BoardId, @UserId, @Role);";

        return _connection.ExecuteAsync(
            sql, 
            new { BoardId = boardId, UserId = userId, Role = role }, 
            _transaction);
    }

    public async Task<bool> IsMemberAsync(Guid boardId, Guid userId)
    {
        const string sql = """
            SELECT COUNT(1) FROM BoardMembers 
            WHERE BoardId = @BoardId AND UserId = @UserId
            """;

        var result = await _connection.QueryFirstOrDefaultAsync<int>(
            sql, 
            new { BoardId = boardId, UserId = userId }, 
            _transaction);

        return result > 0;
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

        return await _connection.QueryAsync<Board>(
            sql, 
            new { UserId = userId });
    }

    public async Task<BoardDetailsDto?> GetDetailsAsync(Guid boardId, Guid userId)
    {
        const string sqlBoardDetails = """
            SELECT
                b.Id,
                b.Name,
                b.IsPublic,
                b.CreatedBy,
                b.CreatedAt,
                bm.[Role] AS UserRole,

                l.Id,
                l.BoardId,
                l.Name,
                l.Position,

                c.Id,
                c.ListId,
                c.Name,
                c.Description,
                c.Position,

                a.Id,
                a.FileName,
                a.BlobUrl,

                u.Id AS UserId,
                u.EmailAddress

            FROM Boards b
            LEFT JOIN BoardMembers bm ON bm.BoardId = b.Id AND bm.UserId = @UserId
            LEFT JOIN Lists l ON l.BoardId = b.Id
            LEFT JOIN Cards c ON c.ListId = l.Id
            LEFT JOIN CardAttachments a ON a.CardId = c.Id
            LEFT JOIN CardAssignees ca ON ca.CardId = c.Id
            LEFT JOIN Users u ON u.Id = ca.UserId

            WHERE b.Id = @BoardId

            ORDER BY l.Position, c.Position;
            """;
        
        const string sqlBoardMembers = """
        SELECT 
            bm.UserId,
            u.EmailAddress,
            bm.[Role]
        FROM BoardMembers bm
        JOIN Users u ON bm.UserId = u.Id
        WHERE bm.BoardId = @BoardId;
        """;


        var boardDictionary = new Dictionary<Guid, BoardDetailsDto>();
        var listDictionary = new Dictionary<Guid, ListDto>();
        var cardDictionary = new Dictionary<Guid, CardDto>();

        var result = await _connection.QueryAsync<
            BoardDetailsDto,
            ListDto,
            CardDto,
            AttachmentResponseDto,
            CardAssigneeDto,
            BoardDetailsDto>(
            sqlBoardDetails,
            (board, list, card, attachment, assignee) =>
            {
                if (!boardDictionary.TryGetValue(board.Id, out var boardEntry))
                {
                    boardEntry = board;
                    boardEntry.Lists = [];
                    boardEntry.Members = [];

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
                        if (!cardDictionary.TryGetValue(card.Id, out var cardEntry))
                        {
                            cardEntry = card;
                            cardEntry.Attachments = [];
                            cardDictionary.Add(cardEntry.Id, cardEntry);
                            listEntry.Cards.Add(cardEntry);
                        }

                        if (attachment is not null && attachment.Id != Guid.Empty)
                        {
                            if (!cardEntry.Attachments.Any(x => x.Id == attachment.Id))
                            {
                                cardEntry.Attachments.Add(attachment);
                            }
                        }

                        if (assignee is not null && assignee.UserId != Guid.Empty)
                        {
                            if (!cardEntry.Assignees.Any(x => x.UserId == assignee.UserId))
                            {
                                cardEntry.Assignees.Add(assignee);
                            }
                        }
                    }
                }

                return boardEntry;
            },
            new { BoardId = boardId, UserId = userId },
            splitOn: "Id,Id,Id, UserId");

        var boardDetails = boardDictionary.Values.FirstOrDefault();
        if (boardDetails is not null)
        {
            var members = await _connection.QueryAsync<BoardMemberDto>(
                sqlBoardMembers,
                new { BoardId = boardId });

            boardDetails.Members = members.ToList();
        }

        return boardDetails;
    }

    public async Task<BoardRole?> GetUserRoleAsync(Guid boardId, Guid userId)
    {
        const string sql = """
            SELECT [Role] 
            FROM BoardMembers 
            WHERE BoardId = @BoardId AND UserId = @UserId;
            """;

        var roleValue = await _connection.QueryFirstOrDefaultAsync<int?>(
            sql, 
            new { BoardId = boardId, UserId = userId }, 
            _transaction);

        if (roleValue is null)
        {
            return null;
        }

        return (BoardRole)roleValue.Value;
    }
}