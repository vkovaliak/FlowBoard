namespace FlowBoard.Domain.Constants;

public static class StripeConstants
{
    public const string WebhookHeader = "Stripe-Signature";
    public const string SubscriptionMode = "subscription";
    public const string MetadataUserIdKey = "userId";
    public const string CheckoutSessionCompleted = "checkout.session.completed";
    public const string SubscriptionDeleted = "customer.subscription.deleted";
}