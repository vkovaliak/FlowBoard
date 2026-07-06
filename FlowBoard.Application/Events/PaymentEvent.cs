namespace FlowBoard.Application.Events;

public class PaymentEvent
{
    public string Type { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? CustomerId { get; set; }
    public string? SubscriptionId { get; set; }
}