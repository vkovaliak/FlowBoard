using FlowBoard.Domain.DTOs.Auth;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.ExternalLogin;

public record ExternalLoginCommand(
    string Provider,
    string ExternalId,
    string Email,
    string UserName) 
    : IRequest<Result<TokenDto>>;