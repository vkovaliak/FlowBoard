using System.Security.Claims;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace FlowBoard.Infrastructure.Auth;

public class ExternalAuthService : IExternalAuthService
{
    private readonly FrontendOptions _frontend;

    public ExternalAuthService(IOptions<FrontendOptions> frontendOptions)
    {
        _frontend = frontendOptions.Value;
    }

    public ExternalUserInfo ExtractUserInfo(ClaimsPrincipal principal)
    {
        var externalId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? principal.FindFirst(ExternalAuthConstants.SubClaim)?.Value
                        ?? string.Empty;

        var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? principal.FindFirst(ExternalAuthConstants.EmailClaim)?.Value
                    ?? principal.FindFirst(ExternalAuthConstants.PreferredUsernameClaim)?.Value
                    ?? string.Empty;

        var name = principal.FindFirst(ExternalAuthConstants.NameClaimType)?.Value
                ?? email;

        return new ExternalUserInfo(externalId, email, name);
    }

    public string BuildCallbackUrl(TokenDto token)
    {
        return $"{_frontend.BaseUrl}{_frontend.ExternalCallbackPath}" +
            $"?accessToken={Uri.EscapeDataString(token.AccessToken)}" +
            $"&refreshToken={Uri.EscapeDataString(token.RefreshToken)}" +
            $"&accessExpiry={Uri.EscapeDataString(token.AccessTokenExpirationTime.ToString("o"))}" +
            $"&refreshExpiry={Uri.EscapeDataString(token.RefreshTokenExpirationTime.ToString("o"))}";
    }

    public string BuildErrorUrl(string error)
    {
        return $"{_frontend.BaseUrl}{_frontend.LoginPath}?error={error}";
    }
}
