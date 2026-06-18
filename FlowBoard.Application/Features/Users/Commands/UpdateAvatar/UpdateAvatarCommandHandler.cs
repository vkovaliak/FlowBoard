using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.UpdateAvatar;

public class UpdateAvatarCommandHandler
    : IRequestHandler<UpdateAvatarCommand, Result<string>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAvatarCommandHandler(
        IFileStorageService fileStorageService,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _fileStorageService = fileStorageService;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<string>> Handle(
        UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.GetId();

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User not found");
            }

            if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
            {
                await _fileStorageService.DeleteAsync(user.AvatarUrl);
            }

            var fileUrl = await _fileStorageService.UploadAsync(
                request.FileStream,
                request.FileName,
                StorageConstants.AvatarsContainer);

            user.AvatarUrl = fileUrl;
            await _userRepository.UpdateAsync(user);

            return Result.Ok(fileUrl);
        }
        catch
        {
            return Result.Fail("An error occurred while updateing the user avatar");
        }
    }
}