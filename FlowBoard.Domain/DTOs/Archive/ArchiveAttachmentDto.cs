namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string BlobUrl { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
    public Guid UploadetBy { get; set; }
}