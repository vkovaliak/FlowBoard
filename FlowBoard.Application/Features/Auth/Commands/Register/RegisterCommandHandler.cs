using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenDto>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IJwtProvider _jwtProvider;

    public RegisterCommandHandler(IUnitOfWorkFactory uowFactory, IJwtProvider jwtProvider)
    {
        _uowFactory = uowFactory;
        _jwtProvider = jwtProvider;
    }

    public async Task<TokenDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var existingUser = await uow.UserRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("User with this email already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                EmailAddress = request.Email,
                PasswordHash = passwordHash,
            };

            await uow.UserRepository.CreateAsync(user);

            var (accessToken, accessTokenExpiry) = _jwtProvider.GenerateAccessToken(user.Id, user.EmailAddress);
            var (refreshToken, refreshTokenExpiry) = _jwtProvider.GenerateRefreshToken();

            var userSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiryTime = refreshTokenExpiry
            };

            await uow.UserSessionRepository.CreateAsync(userSession);
            return new TokenDto (
                accessToken, 
                refreshToken,
                accessTokenExpiry,
                refreshTokenExpiry);

        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}