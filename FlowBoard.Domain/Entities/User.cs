using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.Domain.Entities;

[Table("Users")]
public class User : BaseEntity<Guid>
{
    public required string EmailAddress { get; set; }
    public required string PasswordHash { get; set; }

    [Editable(false)]
    public DateTime SignupDate { get; set; }
}