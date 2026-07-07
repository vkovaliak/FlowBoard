namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveCardDto
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int Position { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public Guid CreatedBy { get; set; }

    public List<ArchiveAttachmentDto> Attachments { get; set; } = [];
    public List<ArchiveAssigneeDto> Assignees { get; set; } = [];
    public List<ArchiveLabelDto> Labels { get; set; } = [];
    public List<ArchiveChecklistItemDto> ChecklistItems { get; set; } = [];
    public List<ArchiveCommentDto> Comments { get; set; } = [];
}