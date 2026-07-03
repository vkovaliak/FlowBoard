using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Attachments;
using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.DTOs.CardAssignee;
using FlowBoard.Domain.DTOs.Cards;
using FlowBoard.Domain.DTOs.Checklists;
using FlowBoard.Domain.DTOs.Labels;
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

    public async Task<IEnumerable<BoardDto>> GetByUserIdAsync(Guid userId, ArchiveStatus status)
    {
        const string sql = @"
            SELECT DISTINCT 
                b.Id, 
                b.Name, 
                b.IsPublic, 
                b.CreatedBy, 
                b.CreatedAt,
                ISNULL(bm.IsFavorite, 0) AS IsFavorite
            FROM Boards b
            LEFT JOIN BoardMembers bm ON b.Id = bm.BoardId AND bm.UserId = @UserId
            WHERE (b.CreatedBy = @UserId 
                   OR bm.UserId = @UserId)
                AND b.ArchiveStatus = @Status
            ORDER BY b.CreatedAt DESC";

        return await _connection.QueryAsync<BoardDto>(
            sql, 
            new { UserId = userId, Status = (int)status });
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
                bm.IsFavorite,
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
                c.DueDate,
                c.IsCompleted,

                a.Id,
                a.FileName,
                a.BlobUrl,

                u.Id AS UserId,
                u.EmailAddress,
                u.UserName,
                u.AvatarUrl

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
                bm.[Role],
                u.UserName,
                u.AvatarUrl
            FROM BoardMembers bm
            JOIN Users u ON bm.UserId = u.Id
            WHERE bm.BoardId = @BoardId;
            """;
        
        const string sqlCardLabels = """
            SELECT 
                cl.CardId,
                l.Id,
                l.Name,
                l.Color
            FROM CardLabels cl
            JOIN Labels l ON l.Id = cl.LabelId
            JOIN Cards c ON c.Id = cl.CardId
            JOIN Lists li ON li.Id = c.ListId
            WHERE li.BoardId = @BoardId;
            """;

        const string sqlChecklistItems = """
            SELECT 
                ci.Id,
                ci.CardId,
                ci.Text,
                ci.IsCompleted,
                ci.Position
            FROM ChecklistItems ci
            JOIN Cards c ON c.Id = ci.CardId
            JOIN Lists li ON li.Id = c.ListId
            WHERE li.BoardId = @BoardId
            ORDER BY ci.Position;
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
                            cardEntry.Labels = [];
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

            var cardLabelsData = await _connection.QueryAsync<dynamic>(
                sqlCardLabels,
                new { BoardId = boardId });
            
            var labelsByCard = cardLabelsData
                .GroupBy(row => (Guid)row.CardId);
            
            foreach (var group in labelsByCard)
            {
                if (cardDictionary.TryGetValue(group.Key, out var cardEntry))
                {
                    cardEntry.Labels = group.Select(row => new LabelDto
                    {
                        Id = row.Id,
                        Name = row.Name,
                        Color = row.Color
                    }).ToList();
                }
            }

            var checklistItems = await _connection.QueryAsync<ChecklistItemDto>(
                sqlChecklistItems, new { BoardId = boardId });

            var checklistLookup = checklistItems
                .GroupBy(x => x.CardId)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var card in boardDetails.Lists.SelectMany(l => l.Cards))
            {
                if (checklistLookup.TryGetValue(card.Id, out var items))
                {
                    card.ChecklistItems = items;
                }
            }

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

    public async Task<bool> RemoveMemberAsync(Guid boardId, Guid userId)
    {
        const string sql = """
            DELETE FROM BoardMembers
            WHERE BoardId = @BoardId AND UserId = @UserId;
            """;

        var affected = await _connection.ExecuteAsync(
            sql,
            new { BoardId = boardId, UserId = userId },
            transaction: _transaction);

        return affected > 0;
    }

    public async Task<bool> ToggleFavoriteAsync(Guid boardId, Guid userId)
    {
        const string sql = """
            UPDATE BoardMembers
            SET IsFavorite = CASE WHEN IsFavorite = 1 THEN 0 ELSE 1 END
            WHERE BoardId = @BoardId AND UserId = @UserId;
            """;

        var affected = await _connection.ExecuteAsync(
            sql,
            new { BoardId = boardId, UserId = userId },
            _transaction);

        return affected > 0;
    }

    public async Task<IEnumerable<BoardArchiveDto>> GetByArchiveStatusAsync(
        ArchiveStatus status)
    {
        const string sql = """
            SELECT 
                Id, 
                Name, 
                IsPublic, 
                CreatedBy, 
                CreatedAt, 
                ArchivedAt
            FROM Boards
            WHERE ArchiveStatus = @Status;
            """;

        return await _connection.QueryAsync<BoardArchiveDto>(
            sql,
            new { Status = (int)status },
            _transaction);
    }

    public async Task UpdateArchiveStatusAsync(Guid boardId, ArchiveStatus status)
    {
        const string sql = """
            UPDATE Boards
            SET ArchiveStatus = @Status
            WHERE Id = @BoardId;
            """;

        await _connection.ExecuteAsync(
            sql,
            new { BoardId = boardId, Status = (int)status },
            _transaction);
    }

    public async Task<BoardArchiveDto?> GetForArchiveAsync(Guid boardId)
    {
        const string sqlBoardDetails = """
            SELECT
                b.Id,
                b.Name,
                b.IsPublic,
                b.CreatedBy,
                b.CreatedAt,
                b.ArchivedAt,

                l.Id,
                l.BoardId,
                l.Name,
                l.Position,

                c.Id,
                c.ListId,
                c.Name,
                c.Description,
                c.Position,
                c.DueDate,
                c.IsCompleted,

                a.Id,
                a.FileName,
                a.BlobUrl,

                u.Id AS UserId,
                u.EmailAddress,
                u.UserName,
                u.AvatarUrl

            FROM Boards b
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
                bm.[Role],
                u.UserName,
                u.AvatarUrl
            FROM BoardMembers bm
            JOIN Users u ON bm.UserId = u.Id
            WHERE bm.BoardId = @BoardId;
            """;

        const string sqlCardLabels = """
            SELECT 
                cl.CardId,
                l.Id,
                l.Name,
                l.Color
            FROM CardLabels cl
            JOIN Labels l ON l.Id = cl.LabelId
            JOIN Cards c ON c.Id = cl.CardId
            JOIN Lists li ON li.Id = c.ListId
            WHERE li.BoardId = @BoardId;
            """;

        const string sqlChecklistItems = """
            SELECT 
                ci.Id,
                ci.CardId,
                ci.Text,
                ci.IsCompleted,
                ci.Position
            FROM ChecklistItems ci
            JOIN Cards c ON c.Id = ci.CardId
            JOIN Lists li ON li.Id = c.ListId
            WHERE li.BoardId = @BoardId
            ORDER BY ci.Position;
            """;

        var boardDictionary = new Dictionary<Guid, BoardArchiveDto>();
        var listDictionary = new Dictionary<Guid, ListDto>();
        var cardDictionary = new Dictionary<Guid, CardDto>();

        await _connection.QueryAsync<
            BoardArchiveDto,
            ListDto,
            CardDto,
            AttachmentResponseDto,
            CardAssigneeDto,
            BoardArchiveDto>(
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
                            cardEntry.Labels = [];
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
            new { BoardId = boardId },
            splitOn: "Id,Id,Id,UserId");

        var boardArchive = boardDictionary.Values.FirstOrDefault();

        if (boardArchive is not null)
        {
            var members = await _connection.QueryAsync<BoardMemberDto>(
                sqlBoardMembers, new { BoardId = boardId });
            boardArchive.Members = members.ToList();

            var cardLabelsData = await _connection.QueryAsync<dynamic>(
                sqlCardLabels, new { BoardId = boardId });

            var labelsByCard = cardLabelsData.GroupBy(row => (Guid)row.CardId);

            foreach (var group in labelsByCard)
            {
                if (cardDictionary.TryGetValue(group.Key, out var cardEntry))
                {
                    cardEntry.Labels = group.Select(row => new LabelDto
                    {
                        Id = row.Id,
                        Name = row.Name,
                        Color = row.Color
                    }).ToList();
                }
            }

            var checklistItems = await _connection.QueryAsync<ChecklistItemDto>(
                sqlChecklistItems, new { BoardId = boardId });

            var checklistLookup = checklistItems
                .GroupBy(x => x.CardId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var card in boardArchive.Lists.SelectMany(l => l.Cards))
            {
                if (checklistLookup.TryGetValue(card.Id, out var items))
                {
                    card.ChecklistItems = items;
                }
            }
        }

        return boardArchive;
    }

    public async Task DeleteBoardContentAsync(Guid boardId)
    {
        const string sql = """
            DELETE FROM Lists WHERE BoardId = @BoardId;
            DELETE FROM Labels WHERE BoardId = @BoardId;
            DELETE FROM BoardMembers WHERE BoardId = @BoardId;
            """;

        await _connection.ExecuteAsync(
            sql, new { BoardId = boardId }, 
            _transaction);
    }

    public async Task<int> GetOwnerCountAsync(Guid boardId)
    {
        const string sql = """
            SELECT COUNT(1) 
            FROM BoardMembers
            WHERE BoardId = @BoardId 
            AND [Role] = @OwnerRole;
            """;

        return await _connection.QueryFirstOrDefaultAsync<int>(
            sql,
            new 
            {  
                BoardId = boardId, 
                OwnerRole = (int)BoardRole.Owner 
            },
            _transaction);
    }

    public async Task<bool> UpdateMemberRoleAsync(
        Guid boardId, Guid userId, BoardRole role)
    {
        const string sql = """
            UPDATE BoardMembers
            SET [Role] = @Role
            WHERE BoardId = @BoardId 
            AND UserId = @UserId;
            """;

        var affected = await _connection.ExecuteAsync(
            sql,
            new 
            { 
                BoardId = boardId, 
                UserId = userId, 
                Role = (int)role 
            },
            _transaction);

        return affected > 0;
    }

    public async Task TransferOwnershipAsync(
        Guid boardId, Guid currentOwnerId, Guid newOwnerId)
    {
        const string sql = """
            UPDATE BoardMembers
            SET [Role] = @AdminRole
            WHERE BoardId = @BoardId 
            AND UserId = @CurrentOwnerId;

            UPDATE BoardMembers
            SET [Role] = @OwnerRole
            WHERE BoardId = @BoardId 
            AND UserId = @NewOwnerId;
            """;

        await _connection.ExecuteAsync(
            sql,
            new
            {
                BoardId = boardId,
                CurrentOwnerId = currentOwnerId,
                NewOwnerId = newOwnerId,
                AdminRole = (int)BoardRole.Admin,
                OwnerRole = (int)BoardRole.Owner
            },
            _transaction);
    }
}