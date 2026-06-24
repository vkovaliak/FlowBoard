using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IUserRepository userRepository, IUserSessionRepository userSessionRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _userSessionRepository = userSessionRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<TokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            return Result.Fail("Invalid email.");
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return Result.Fail("This account uses external sign-in. Please use Microsoft.");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result.Fail("Invalid password.");
        }
        
        var (accessToken, accessTokenExpiry) = _jwtProvider.GenerateAccessToken(user.Id, user.EmailAddress);
        var (refreshToken, refreshTokenExpiry) = _jwtProvider.GenerateRefreshToken();

        var userSession = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiryTime = refreshTokenExpiry,

        };

        await _userSessionRepository.CreateAsync(userSession);

        var tokenDto = new TokenDto(
            accessToken, 
            refreshToken,
            accessTokenExpiry,
            refreshTokenExpiry
        );

        return Result.Ok(tokenDto);
    }
}