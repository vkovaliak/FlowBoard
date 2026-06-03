using FlowBoard.Domain.DTOs.Auth;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken) 
    : IRequest<Result<TokenDto>>;