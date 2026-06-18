using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Users;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        var user = await _userRepository.GetByIdAsync(currentUserId);
        if (user is null)
        {
            return Result.Fail("User not found");
        }

        var dto = new UserDto(
            user.Id,
            user.EmailAddress,
            user.UserName,
            user.AvatarUrl);

        return Result.Ok(dto);
    }
}