using FluentResults;
using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler 
    : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(
        ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetId();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Result.Fail("User not found.");
        }

        if (!string.IsNullOrEmpty(user.ExternalProvider))
        {
            return Result.Fail("Password change is not available for external accounts.");
        }

        var isCurrentValid = BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword, user.PasswordHash);

        if (!isCurrentValid)
        {
            return Result.Fail("Current password is incorrect.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _userRepository.UpdatePasswordAsync(user.Id, user.PasswordHash);

        return Result.Ok(true);
    }
}