using FlowBoard.Domain.DTOs.Attachments;

namespace FlowBoard.Domain.DTOs.Comments;

public record CommentDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public string Email { get; set; } = default!;
    public required string UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public IReadOnlyList<AttachmentResponseDto> Attachments { get; set; } = default!;
};