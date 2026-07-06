using FlowBoard.Application.Abstractions;
using FlowBoard.Application.Events;
using FlowBoard.Domain.Constants;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace FlowBoard.Infrastructure.Payments;

public class StripePaymentService : IPaymentService
{
    private readonly StripeOptions _options;

    public StripePaymentService(IOptions<StripeOptions> options)
    {
        _options = options.Value;

        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        Guid userId, string email, string? existingCustomerId)
    {
        var options = new SessionCreateOptions
        {
            Mode = StripeConstants.SubscriptionMode,

            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = _options.ProPriceId,
                    Quantity = 1
                }
            ],

            SuccessUrl = _options.SuccessUrl,
            CancelUrl = _options.CancelUrl,

            ClientReferenceId = userId.ToString(),

            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    [StripeConstants.MetadataUserIdKey] = userId.ToString()
                }
            }
        };

        if (!string.IsNullOrEmpty(existingCustomerId))
        {
            options.Customer = existingCustomerId;
        }
        else
        {
            options.CustomerEmail = email;
        }

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public PaymentEvent? ParseWebhookEvent(
        string payload, string signature)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                payload,
                signature,
                _options.WebhookSecret);
        }
        catch (StripeException)
        {
            return null;
        }

        var result = new PaymentEvent
        {
            Type = stripeEvent.Type
        };

        switch (stripeEvent.Type)
        {
            case StripeConstants.CheckoutSessionCompleted:
            {
                var session = stripeEvent.Data.Object as Session;

                if (session is not null)
                {
                    if (Guid.TryParse(
                        session.ClientReferenceId, out var userId))
                    {
                        result.UserId = userId;
                    }

                    result.CustomerId = session.CustomerId;
                    result.SubscriptionId = session.SubscriptionId;
                }

                break;
            }

            case StripeConstants.SubscriptionDeleted:
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                if (subscription is not null)
                {
                    result.CustomerId = subscription.CustomerId;
                    result.SubscriptionId = subscription.Id;

                    if (subscription.Metadata.TryGetValue(
                        StripeConstants.MetadataUserIdKey, out var uid)
                        && Guid.TryParse(uid, out var userId))
                    {
                        result.UserId = userId;
                    }
                }

                break;
            }
        }

        return result;
    }
}