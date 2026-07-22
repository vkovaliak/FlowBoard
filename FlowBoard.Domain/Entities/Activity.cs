using System.ComponentModel.DataAnnotations.Schema;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

[Table("Activities")]
public class Activity : BaseEntity<Guid>
{
    public Guid CardId { get; set; }
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public ActivityAction ActionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}