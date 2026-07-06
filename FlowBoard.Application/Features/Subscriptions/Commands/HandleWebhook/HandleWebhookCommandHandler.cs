using FlowBoard.Application.Abstractions;
using FlowBoard.Application.Events;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Enums;
using MediatR;

namespace FlowBoard.Application.Features.Subscriptions.Commands.HandleWebhook;

public class HandleWebhookCommandHandler
    : IRequestHandler<HandleWebhookCommand>
{
    private readonly IUserRepository _userRepository;

    public HandleWebhookCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(
        HandleWebhookCommand request, CancellationToken cancellationToken)
    {
        var evt = request.Event;

        switch (evt.Type)
        {
            case StripeConstants.CheckoutSessionCompleted:
                await ActivateProAsync(evt);
                break;

            case StripeConstants.SubscriptionDeleted:
                await DeactivateAsync(evt);
                break;
        }
    }

    private async Task ActivateProAsync(PaymentEvent evt)
    {
        if (evt.UserId is null)
        {
            return;
        }

        var user = await _userRepository.GetByIdAsync(
            evt.UserId.Value);
        if (user is null)
        {
            return;
        }

        if (user.SubscriptionPlan == SubscriptionPlan.Pro)
        {
            return;
        }

        user.SubscriptionPlan = SubscriptionPlan.Pro;
        user.StripeCustomerId = evt.CustomerId;
        user.StripeSubscriptionId = evt.SubscriptionId;

        await _userRepository.UpdateAsync(user);
    }

    private async Task DeactivateAsync(PaymentEvent evt)
    {
        User? user = null;

        if (evt.UserId is not null)
        {
            user = await _userRepository.GetByIdAsync(evt.UserId.Value);
        }

        if (user is null && !string.IsNullOrEmpty(evt.CustomerId))
        {
            user = await _userRepository.GetByStripeCustomerIdAsync(
                evt.CustomerId);
        }

        if (user is null)
        {
            return;
        }

        user.SubscriptionPlan = SubscriptionPlan.None;
        user.StripeSubscriptionId = null;

        await _userRepository.UpdateAsync(user);
    }
}