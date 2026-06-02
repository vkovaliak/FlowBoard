using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenDto>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenCommandHandler(IUnitOfWorkFactory uowFactory, IJwtProvider jwtProvider)
    {
        _uowFactory = uowFactory;
        _jwtProvider = jwtProvider;
    }

    public async Task<TokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var session = await uow.UserSessionRepository.GetByTokenAsync(request.RefreshToken);
            if (session == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (session.ExpiryTime < DateTime.UtcNow)
            {
                await uow.UserSessionRepository.DeleteAsync(session);
                throw new UnauthorizedAccessException("Refresh token expired. Please login again.");
            }

            var user = await uow.UserRepository.GetByIdAsync(session.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var newAccessToken = _jwtProvider.GenerateAccessToken(user.Id, user.EmailAddress);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken();

            await uow.UserSessionRepository.DeleteAsync(session);

            var newSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiryTime = DateTime.UtcNow.AddDays(7)
            };
            await uow.UserSessionRepository.CreateAsync(newSession);

            uow.Commit();

            return new TokenDto(newAccessToken, newRefreshToken);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}