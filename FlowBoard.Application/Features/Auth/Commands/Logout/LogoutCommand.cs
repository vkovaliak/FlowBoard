using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(
    string RefreshToken) 
    : IRequest<Result<bool>>;