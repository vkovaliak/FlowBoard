namespace FlowBoard.Domain.DTOs.Attachments;

public class AttachmentResponseDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string BlobUrl { get; set; } = default!;
}