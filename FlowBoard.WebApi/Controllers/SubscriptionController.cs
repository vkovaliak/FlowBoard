using FlowBoard.Application.Abstractions;
using FlowBoard.Application.Features.Subscriptions.Commands.CreateCheckout;
using FlowBoard.Application.Features.Subscriptions.Commands.HandleWebhook;
using FlowBoard.Application.Features.Users.Commands.CancelSubscription;
using FlowBoard.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/subscription")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPaymentService _paymentService;

    public SubscriptionController(
        IMediator mediator, IPaymentService paymentService)
    {
        _mediator = mediator;
        _paymentService = paymentService;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout()
    {
        var result = await _mediator.Send(
            new CreateCheckoutCommand());

       if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);

        var payload = await reader.ReadToEndAsync();

        var signature = Request.Headers[
            StripeConstants.WebhookHeader].ToString();

        var paymentEvent = _paymentService.ParseWebhookEvent(
            payload, signature);

        if (paymentEvent is null)
        {
            return BadRequest();
        }

        await _mediator.Send(new HandleWebhookCommand(
            paymentEvent));

        return Ok();
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscriptionAsync()
    {
        var command = new CancelSubscriptionCommand();
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}