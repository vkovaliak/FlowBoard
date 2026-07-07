namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveChecklistItemDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Text { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public int Position { get; set; }
    public Guid CreatedBy { get; set; }
}