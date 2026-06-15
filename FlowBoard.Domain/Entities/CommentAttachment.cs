using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("CommentAttachment")]
public class CommentAttachment : BaseEntity<Guid>
{
    public Guid CommentId { get; set; }

    public required string FileName { get; set; }

    public required string BlobUrl { get; set; }

    public required string ContentType { get; set; }

    [Editable(false)]
    public DateTime UploadedAt { get; set; }

    public Guid UploadedBy { get; set; }
}