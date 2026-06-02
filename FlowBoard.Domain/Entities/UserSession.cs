using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("UserSessions")]
public class UserSession : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiryTime { get; set; }
}