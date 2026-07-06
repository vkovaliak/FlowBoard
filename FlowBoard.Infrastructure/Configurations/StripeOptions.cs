namespace FlowBoard.Infrastructure.Configurations;

public class StripeOptions
{
    public const string SectionName = "StripeOptions";
    
    public string SecretKey { get; set; } = string.Empty;
    public string ProPriceId { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;

    public string WebhookSecret { get; set; } = string.Empty;

    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}