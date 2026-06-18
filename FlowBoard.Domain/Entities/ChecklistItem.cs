using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("ChecklistItems")]
public class ChecklistItem : BaseEntity<Guid>
{
    public Guid CardId { get; set; }

    public required string Text { get; set; }

    public bool IsCompleted { get; set; }

    public int Position { get; set; }

    [Editable(false)]
    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
}