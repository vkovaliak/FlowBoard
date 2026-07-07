namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveCommentDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}