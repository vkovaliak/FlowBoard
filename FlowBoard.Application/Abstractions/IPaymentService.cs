using FlowBoard.Application.Events;

namespace FlowBoard.Application.Abstractions;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(
        Guid userId, string email, string? existingCustomerId);

    PaymentEvent? ParseWebhookEvent(string payload, string signature);
}