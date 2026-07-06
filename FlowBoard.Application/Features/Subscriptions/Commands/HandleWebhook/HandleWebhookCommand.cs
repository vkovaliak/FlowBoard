using FlowBoard.Application.Events;
using MediatR;

namespace FlowBoard.Application.Features.Subscriptions.Commands.HandleWebhook;

public record HandleWebhookCommand(
    PaymentEvent Event) 
    : IRequest;