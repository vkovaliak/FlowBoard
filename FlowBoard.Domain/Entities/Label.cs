using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("Labels")]
public class Label : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }

    public required string Name { get; set; }

    public required string Color { get; set; }

    [Editable(false)]
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
}