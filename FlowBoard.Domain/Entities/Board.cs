using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

[Table("Boards")]
public class Board : BaseEntity<Guid>
{
    public required string Name { get; set; }
    
    public bool IsPublic { get; set; }
    
    [Editable(false)]
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }

    public ArchiveStatus ArchiveStatus { get; set; } = ArchiveStatus.None;
    
    public DateTime? ArchivedAt { get; set; }

    public string? Background { get; set; }
}