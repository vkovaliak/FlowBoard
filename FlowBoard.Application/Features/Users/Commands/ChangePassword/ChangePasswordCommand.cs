using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword) 
    : IRequest<Result<bool>>;