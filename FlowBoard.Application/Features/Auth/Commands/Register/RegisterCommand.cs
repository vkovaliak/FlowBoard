using FlowBoard.Domain.DTOs.Auth;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email, 
    string Password,
    string UserName) 
    : IRequest<Result<TokenDto>>;