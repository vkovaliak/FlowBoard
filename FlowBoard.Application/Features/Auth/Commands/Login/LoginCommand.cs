using MediatR;
using FlowBoard.Domain.DTOs.Auth;
using FluentResults;

namespace FlowBoard.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) 
    : IRequest<Result<TokenDto>>;