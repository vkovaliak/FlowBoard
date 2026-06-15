namespace FlowBoard.Domain.DTOs.Attachments;

public record AttachmentResponse(
    Guid Id,
    string FileName,
    string BlobUrl);