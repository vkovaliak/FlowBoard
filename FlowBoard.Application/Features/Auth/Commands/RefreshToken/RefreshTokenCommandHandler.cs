using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenCommandHandler(IUnitOfWorkFactory uowFactory, IJwtProvider jwtProvider)
    {
        _uowFactory = uowFactory;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var session = await uow.UserSessionRepository.GetByTokenAsync(request.RefreshToken);
            if (session is null)
            {
                return Result.Fail("Invalid refresh token.");
            }

            if (session.ExpiryTime < DateTime.UtcNow)
            {
                await uow.UserSessionRepository.DeleteAsync(session);
                return Result.Fail("Refresh token expired. Please login again.");
            }

            var user = await uow.UserRepository.GetByIdAsync(session.UserId);
            if (user is null)
            {
                return Result.Fail("User not found.");
            }

            var (newAccessToken, accessTokenExpiry) = _jwtProvider.GenerateAccessToken(user.Id, user.EmailAddress);
            var (newRefreshToken, refreshTokenExpiry) = _jwtProvider.GenerateRefreshToken();

            await uow.UserSessionRepository.DeleteAsync(session);

            var newSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiryTime = refreshTokenExpiry
            };
            await uow.UserSessionRepository.CreateAsync(newSession);

            uow.Commit();

            var tokenDto =  new TokenDto(
                newAccessToken, 
                newRefreshToken,
                accessTokenExpiry,
                refreshTokenExpiry
            );

            return Result.Ok(tokenDto);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}