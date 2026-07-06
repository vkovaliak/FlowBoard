using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Subscriptions.Commands.CreateCheckout;

public record CreateCheckoutCommand 
    : IRequest<Result<string>>;