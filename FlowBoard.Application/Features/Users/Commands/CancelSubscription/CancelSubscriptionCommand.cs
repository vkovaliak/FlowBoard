using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.CancelSubscription;

public record CancelSubscriptionCommand 
    : IRequest<Result<bool>>;