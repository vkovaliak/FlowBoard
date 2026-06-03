using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
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

    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email) 
            ?? throw new UnauthorizedAccessException("Invalid email.");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid password.");
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

        return new TokenDto(
            accessToken, 
            refreshToken,
            accessTokenExpiry,
            refreshTokenExpiry
        );
    }
}