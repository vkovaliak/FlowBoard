using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("Cards")]
public class Card : BaseEntity<Guid>
{
    public Guid ListId { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }

    public required int Position { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    [Editable(false)]
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
}