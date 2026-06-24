using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.DeleteAvatar;

public class DeleteAvatarCommandHandler
    : IRequestHandler<DeleteAvatarCommand, Result<bool>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAvatarCommandHandler(
        IFileStorageService fileStorageService,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _fileStorageService = fileStorageService;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        DeleteAvatarCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.GetId();

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User not found");
            }

            if (string.IsNullOrWhiteSpace(user.AvatarUrl))
            {
                return Result.Fail("User has no avatar to delete.");
            }

            await _fileStorageService.DeleteAsync(user.AvatarUrl);

            user.AvatarUrl = null;
            await _userRepository.UpdateAsync(user);

            return Result.Ok(true);
        }
        catch
        {
            return Result.Fail(
                "An error occurred while deleting the user avatar");
        }
    }
}