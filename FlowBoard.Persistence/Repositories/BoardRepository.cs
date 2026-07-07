using System.Data;
using Dapper;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Archive;
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
        const string sqlBoards = """
            SELECT DISTINCT 
                b.Id, 
                b.Name, 
                b.IsPublic,
                b.Background, 
                b.CreatedBy, 
                b.CreatedAt,
                ISNULL(bm.IsFavorite, 0) AS IsFavorite,
                ISNULL(bm.Role, 0) AS UserRole
            FROM Boards b
            LEFT JOIN BoardMembers bm ON b.Id = bm.BoardId AND bm.UserId = @UserId
            WHERE (b.CreatedBy = @UserId 
                OR bm.UserId = @UserId)
                AND b.ArchiveStatus = @Status
            ORDER BY b.CreatedAt DESC
            """;

        var boards = (await _connection.QueryAsync<BoardDto>(
            sqlBoards,
            new { UserId = userId, Status = (int)status }))
            .ToList();

        if (boards.Count == 0)
        {
            return boards;
        }

        var boardIds = boards.Select(b => b.Id).ToArray();

        const string sqlMembers = @"
            SELECT 
                bm.BoardId,
                bm.UserId,
                u.UserName,
                u.AvatarUrl
            FROM BoardMembers bm
            JOIN Users u ON u.Id = bm.UserId
            WHERE bm.BoardId IN @BoardIds";

        var membersRaw = await _connection.QueryAsync<(
            Guid BoardId, Guid UserId, string UserName, string? AvatarUrl)>(
            sqlMembers,
            new { BoardIds = boardIds });

        var membersByBoard = membersRaw
            .GroupBy(m => m.BoardId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => new BoardMemberAvatarDto(
                     m.UserId, m.UserName, m.AvatarUrl)).ToList());

        foreach (var board in boards)
        {
            if (membersByBoard.TryGetValue(board.Id, out var members))
            {
                board.Members = members;
            }
        }

        return boards;
    }

    public async Task<BoardDetailsDto?> GetDetailsAsync(Guid boardId, Guid userId)
    {
        const string sqlBoardDetails = """
            SELECT
                b.Id,
                b.Name,
                b.IsPublic,
                b.Background,
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

        const string sqlOwnerPlan = """
            SELECT u.SubscriptionPlan
            FROM Boards b
            JOIN Users u ON u.Id = b.CreatedBy
            WHERE b.Id = @BoardId;
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
            var ownerPlan = await _connection.QueryFirstOrDefaultAsync<int?>(
                sqlOwnerPlan,
                new { BoardId = boardId });
            
            boardDetails.OwnerIsPro =
                ownerPlan.HasValue
                && ownerPlan.Value == (int)SubscriptionPlan.Pro;


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
                l.CreatedBy,

                c.Id,
                c.ListId,
                c.Name,
                c.Description,
                c.Position,
                c.DueDate,
                c.IsCompleted,
                c.CreatedBy,

                a.Id,
                a.FileName,
                a.BlobUrl,
                a.ContentType,
                a.UploadedAt,
                a.UploadetBy AS UploadetBy,

                ca.UserId

            FROM Boards b
            LEFT JOIN Lists l ON l.BoardId = b.Id
            LEFT JOIN Cards c ON c.ListId = l.Id
            LEFT JOIN CardAttachments a ON a.CardId = c.Id
            LEFT JOIN CardAssignees ca ON ca.CardId = c.Id

            WHERE b.Id = @BoardId

            ORDER BY l.Position, c.Position;
            """;

        const string sqlBoardMembers = """
            SELECT bm.UserId, bm.[Role]
            FROM BoardMembers bm
            WHERE bm.BoardId = @BoardId;
            """;

        const string sqlCardLabels = """
            SELECT 
                cl.CardId,
                l.Id,
                l.Name,
                l.Color,
                l.CreatedBy
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
                ci.Position,
                ci.CreatedBy
            FROM ChecklistItems ci
            JOIN Cards c ON c.Id = ci.CardId
            JOIN Lists li ON li.Id = c.ListId
            WHERE li.BoardId = @BoardId
            ORDER BY ci.Position;
            """;

        const string sqlComments = """
            SELECT 
                cm.Id, 
                cm.CardId, 
                cm.Message, 
                cm.CreatedAt, 
                cm.CreatedBy
            FROM Comments cm
            JOIN Cards c ON c.Id = cm.CardId
            JOIN Lists li ON li.Id = c.ListId
            WHERE li.BoardId = @BoardId
            ORDER BY cm.CreatedAt;
            """;

        var boardDictionary = new Dictionary<Guid, BoardArchiveDto>();
        var listDictionary = new Dictionary<Guid, ArchiveListDto>();
        var cardDictionary = new Dictionary<Guid, ArchiveCardDto>();

        await _connection.QueryAsync<
            BoardArchiveDto,
            ArchiveListDto,
            ArchiveCardDto,
            ArchiveAttachmentDto,
            ArchiveAssigneeDto,
            BoardArchiveDto>(
            sqlBoardDetails,
            (board, list, card, attachment, assignee) =>
            {
                if (!boardDictionary.TryGetValue(board.Id, out var boardEntry))
                {
                    boardEntry = board;
                    boardDictionary.Add(boardEntry.Id, boardEntry);
                }

                if (list is not null)
                {
                    if (!listDictionary.TryGetValue(list.Id, out var listEntry))
                    {
                        listEntry = list;
                        listDictionary.Add(listEntry.Id, listEntry);
                        boardEntry.Lists.Add(listEntry);
                    }

                    if (card is not null)
                    {
                        if (!cardDictionary.TryGetValue(card.Id, out var cardEntry))
                        {
                            cardEntry = card;
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

        if (boardArchive is null)
        {
            return null;
        }

        var members = await _connection.QueryAsync<ArchiveMemberDto>(
            sqlBoardMembers, new { BoardId = boardId });

        boardArchive.Members = members.ToList();

        var cardLabelsData = await _connection.QueryAsync<dynamic>(
            sqlCardLabels, new { BoardId = boardId });

        foreach (var group in cardLabelsData.GroupBy(
            row => (Guid)row.CardId))
        {
            if (cardDictionary.TryGetValue(group.Key, out var cardEntry))
            {
                cardEntry.Labels = group.Select(
                    row => new ArchiveLabelDto
                {
                    Id = row.Id,
                    Name = row.Name,
                    Color = row.Color,
                    CreatedBy = row.CreatedBy
                }).ToList();
            }
        }

        var checklistItems = await _connection.QueryAsync<ArchiveChecklistItemDto>(
            sqlChecklistItems, new { BoardId = boardId });

        foreach (var group in checklistItems.GroupBy(x => x.CardId))
        {
            if (cardDictionary.TryGetValue(group.Key, out var cardEntry))
            {
                cardEntry.ChecklistItems = group.ToList();
            }
        }


        var comments = (await _connection.QueryAsync<ArchiveCommentDto>(
            sqlComments, new { BoardId = boardId })).ToList();

        foreach (var group in comments.GroupBy(x => x.CardId))
        {
            if (cardDictionary.TryGetValue(
                group.Key, out var cardEntry))
            {
                cardEntry.Comments = group.ToList();
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

    public async Task<int> GetOwnedBoardsCountAsync(Guid userId)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM Boards
            WHERE CreatedBy = @UserId
            AND ArchiveStatus = @Active;
            """;

        return await _connection.QueryFirstOrDefaultAsync<int>(
            sql,
            new { 
                UserId = userId, 
                Active = (int)ArchiveStatus.None 
            },
            _transaction);
    }

    public async Task<int> GetMembersCountAsync(Guid boardId)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM BoardMembers
            WHERE BoardId = @BoardId;
            """;

        return await _connection.QueryFirstOrDefaultAsync<int>(
            sql,
            new { BoardId = boardId },
            _transaction);
    }

    public async Task RestoreBoardContentAsync(BoardArchiveDto board)
    {
        if (board.Members.Count > 0)
        {
            const string sqlMembers = """
                INSERT INTO BoardMembers 
                    (BoardId, UserId, Role, IsFavorite)
                VALUES 
                    (@BoardId, @UserId, @Role, 0);
                """;

            foreach (var member in board.Members)
            {
                await _connection.ExecuteAsync(
                    sqlMembers,
                    new { BoardId = board.Id, member.UserId, Role = (int)member.Role },
                    _transaction);
            }
        }

        var allLabels = board.Lists
            .SelectMany(l => l.Cards)
            .SelectMany(c => c.Labels)
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .ToList();

        if (allLabels.Count > 0)
        {
            const string sqlLabel = """
                INSERT INTO Labels 
                    (Id, BoardId, Name, Color, CreatedBy)
                VALUES 
                    (@Id, @BoardId, @Name, @Color, @CreatedBy);
                """;

            foreach (var label in allLabels)
            {
                await _connection.ExecuteAsync(
                    sqlLabel,
                    new 
                    { 
                        label.Id, 
                        BoardId = board.Id, 
                        label.Name, 
                        label.Color, 
                        label.CreatedBy 
                    },
                    _transaction);
            }
        }

        const string sqlList = """
            INSERT INTO Lists 
                (Id, BoardId, Name, Position, CreatedBy)
            VALUES 
                (@Id, @BoardId, @Name, @Position, @CreatedBy);
            """;

        const string sqlCard = """
            INSERT INTO Cards 
                (Id, ListId, Name, Description, Position, DueDate, IsCompleted, CreatedBy)
            VALUES 
                (@Id, @ListId, @Name, @Description, @Position, @DueDate, @IsCompleted, @CreatedBy);
            """;

        const string sqlAttachment = """
            INSERT INTO CardAttachments 
                (Id, CardId, FileName, BlobUrl, ContentType, UploadedAt, UploadetBy)
            VALUES 
                (@Id, @CardId, @FileName, @BlobUrl, @ContentType, @UploadedAt, @UploadetBy);
            """;

        const string sqlAssignee = """
            INSERT INTO CardAssignees (CardId, UserId)
            VALUES (@CardId, @UserId);
            """;

        const string sqlChecklist = """
            INSERT INTO ChecklistItems 
                (Id, CardId, Text, IsCompleted, Position, CreatedBy)
            VALUES 
                (@Id, @CardId, @Text, @IsCompleted, @Position, @CreatedBy);
            """;

        const string sqlCardLabel = """
            INSERT INTO CardLabels (CardId, LabelId)
            VALUES (@CardId, @LabelId);
            """;

        const string sqlComment = """
            INSERT INTO Comments 
                (Id, CardId, Message, CreatedAt, CreatedBy)
            VALUES 
                (@Id, @CardId, @Message, @CreatedAt, @CreatedBy);
            """;

        foreach (var list in board.Lists)
        {
            await _connection.ExecuteAsync(
                sqlList,
                new 
                {
                    list.Id, 
                    BoardId = board.Id, 
                    list.Name, 
                    list.Position, 
                    list.CreatedBy 
                },
                _transaction);

            foreach (var card in list.Cards)
            {
                await _connection.ExecuteAsync(
                    sqlCard,
                    new
                    {
                        card.Id,
                        ListId = list.Id,
                        card.Name,
                        card.Description,
                        card.Position,
                        card.DueDate,
                        card.IsCompleted,
                        card.CreatedBy
                    },
                    _transaction);

                foreach (var att in card.Attachments)
                {
                    await _connection.ExecuteAsync(
                        sqlAttachment,
                        new
                        {
                            att.Id,
                            CardId = card.Id,
                            att.FileName,
                            att.BlobUrl,
                            att.ContentType,
                            att.UploadedAt,
                            att.UploadetBy
                        },
                        _transaction);
                }

                foreach (var assignee in card.Assignees)
                {
                    await _connection.ExecuteAsync(
                        sqlAssignee,
                        new { CardId = card.Id, assignee.UserId },
                        _transaction);
                }

                foreach (var item in card.ChecklistItems)
                {
                    await _connection.ExecuteAsync(
                        sqlChecklist,
                        new
                        {
                            item.Id,
                            CardId = card.Id,
                            item.Text,
                            item.IsCompleted,
                            item.Position,
                            item.CreatedBy
                        },
                        _transaction);
                }

                foreach (var label in card.Labels)
                {
                    await _connection.ExecuteAsync(
                        sqlCardLabel,
                        new { CardId = card.Id, LabelId = label.Id },
                        _transaction);
                }

                foreach (var comment in card.Comments)
                {
                    await _connection.ExecuteAsync(
                        sqlComment,
                        new
                        {
                            comment.Id,
                            CardId = card.Id,
                            comment.Message,
                            comment.CreatedAt,
                            comment.CreatedBy
                        },
                        _transaction);
                }
            }
        }
    }
}