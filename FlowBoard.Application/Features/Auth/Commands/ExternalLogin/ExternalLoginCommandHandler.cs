using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.ExternalLogin;

public class ExternalLoginCommandHandler
    : IRequestHandler<ExternalLoginCommand, Result<TokenDto>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IJwtProvider _jwtProvider;

    public ExternalLoginCommandHandler(
        IUnitOfWorkFactory uowFactory, IJwtProvider jwtProvider)
    {
        _uowFactory = uowFactory;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<TokenDto>> Handle(
        ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        try
        {
            var user = await uow.UserRepository.GetByExternalIdAsync(
                request.Provider, request.ExternalId);

            if (user is null)
            {
                user = await uow.UserRepository.GetByEmailAsync(request.Email);

                if (user is not null)
                {
                    user.ExternalProvider = request.Provider;
                    user.ExternalId = request.ExternalId;
                    await uow.UserRepository.UpdateAsync(user);
                }
            }

            if (user is null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = request.Email,
                    UserName = request.UserName,
                    PasswordHash = null,
                    ExternalProvider = request.Provider,
                    ExternalId = request.ExternalId
                };
                await uow.UserRepository.CreateAsync(user);
            }

            var (accessToken, accessExpiry) = _jwtProvider.GenerateAccessToken(
                user.Id, user.EmailAddress);

            var (refreshToken, refreshExpiry) = _jwtProvider.GenerateRefreshToken();

            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiryTime = refreshExpiry
            };
            await uow.UserSessionRepository.CreateAsync(session);

            uow.Commit();

            var dto = new TokenDto(
                accessToken, refreshToken, accessExpiry, refreshExpiry);

            return Result.Ok(dto);
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}