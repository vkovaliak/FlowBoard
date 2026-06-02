using MediatR;
using FlowBoard.Domain.DTOs.Auth;

namespace FlowBoard.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) 
    : IRequest<TokenDto>;