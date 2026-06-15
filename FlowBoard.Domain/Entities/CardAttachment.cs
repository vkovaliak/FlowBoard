using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("CardAttachment")]
public class CardAttachment : BaseEntity<Guid>
{
    public Guid CardId { get; set; }

    public required string FileName { get; set; }

    public required string BlobUrl { get; set; }

    public required string ContentType { get; set; }

    [Editable(false)]
    public DateTime UploadedAt { get; set; }

    public Guid UploadedBy { get; set; }
}