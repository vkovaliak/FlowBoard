using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

[Table("Users")]
public class User : BaseEntity<Guid>
{
    public required string EmailAddress { get; set; }

    public required string UserName { get; set; }

    public string? PasswordHash { get; set; }

    [Editable(false)]
    public DateTime SignupDate { get; set; }

    public string? AvatarUrl { get; set; }

    public string? ExternalProvider { get; set; }

    public string? ExternalId { get; set; }

    public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.None;

    public string? StripeCustomerId { get; set; }
    
    public string? StripeSubscriptionId { get; set; }
}