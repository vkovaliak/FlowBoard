namespace FlowBoard.Domain.DTOs.Comments;

public record CommentDto(
    Guid Id,
    Guid CardId,
    string Message,
    DateTime CreatedAt,
    Guid CreatedBy,
    string Email
);