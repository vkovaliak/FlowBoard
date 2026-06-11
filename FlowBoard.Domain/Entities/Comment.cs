using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("Comments")]
public class Comment : BaseEntity<Guid>
{
    public Guid CardId { get; set; }

    public required string Message { get; set; }
    
    [Editable(false)]
    public DateTime CreatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
}