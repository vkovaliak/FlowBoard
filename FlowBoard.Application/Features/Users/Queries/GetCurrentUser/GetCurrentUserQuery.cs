using FlowBoard.Domain.DTOs.Users;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery 
    : IRequest<Result<UserDto>>;