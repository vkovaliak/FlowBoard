using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.UpdateUserName;

public record UpdateUserNameCommand(
    string UserName)
    : IRequest<Result<bool>>;