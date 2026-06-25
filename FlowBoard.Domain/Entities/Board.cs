using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public bool IsFavorite { get; set; }
}