using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("Lists")]
public class ListEntity : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }
    
    public required string Name { get; set; }

    public required int Position { get; set; }

    [Editable(false)]
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
}