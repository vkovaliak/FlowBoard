using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler
    : IRequestHandler<CancelSubscriptionCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CancelSubscriptionCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        CancelSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        var user = await _userRepository.GetByIdAsync(currentUserId);
        if (user is null)
        {
            return Result.Fail("User is not found");
        }

        if (user.SubscriptionPlan != SubscriptionPlan.Pro)
        {
            return Result.Fail("You don't have an active Pro subscription");
        }

        var updated = await _userRepository.CancelSubscriptionPlanAsync(
            currentUserId);

        if (!updated)
        {
            return Result.Fail("Failed to cancel subscription");
        }

        return Result.Ok(true);
    }
}