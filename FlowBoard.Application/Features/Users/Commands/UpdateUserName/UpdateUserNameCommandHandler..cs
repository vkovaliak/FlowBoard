using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.UpdateUserName;

public class UpdateUserNameCommandHandler
    : IRequestHandler<UpdateUserNameCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserNameCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        UpdateUserNameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.GetId();

            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User not found");
            }

            user.UserName = request.UserName;
            await _userRepository.UpdateAsync(user);

            return Result.Ok(true);
        }
        catch
        {
            return Result.Fail("An error occurred while updating user name");
        }
    }
}