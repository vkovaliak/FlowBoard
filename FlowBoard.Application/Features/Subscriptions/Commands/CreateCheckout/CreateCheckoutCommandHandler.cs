using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Subscriptions.Commands.CreateCheckout;

public class CreateCheckoutCommandHandler
    : IRequestHandler<CreateCheckoutCommand, Result<string>>
{
    private readonly IPaymentService _paymentService;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateCheckoutCommandHandler(
        IPaymentService paymentService,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<string>> Handle(
        CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetId();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Result.Fail("User not found");
        }

        if (user.SubscriptionPlan == SubscriptionPlan.Pro)
        {
            return Result.Fail("You already have a Pro subscription");
        }

        var checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(
            userId,
            user.EmailAddress,
            user.StripeCustomerId);

        return checkoutUrl;
    }
}