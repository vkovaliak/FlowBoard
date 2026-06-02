using FlowBoard.Domain.DTOs.Auth;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken) 
    : IRequest<TokenDto>;