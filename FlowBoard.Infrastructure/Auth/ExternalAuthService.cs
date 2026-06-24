using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.DTOs.Auth;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace FlowBoard.Infrastructure.Auth;

public class ExternalAuthService : IExternalAuthService
{
    private readonly EntraIdOptions _entra;
    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configManager;

    public ExternalAuthService(IOptions<EntraIdOptions> entraOptions)
    {
        _entra = entraOptions.Value;

        var metadataAddress = $"{_entra.Authority}{ExternalAuthConstants.OpenIdConfigurationPath}";

        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever());
    }

    public async Task<ClaimsPrincipal?> ValidateMicrosoftTokenAsync(string token)
    {
        try
        {
            var config = await _configManager.GetConfigurationAsync();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidAudience = _entra.ClientId,
                ValidateLifetime = true,
                IssuerSigningKeys = config.SigningKeys,
                NameClaimType = ExternalAuthConstants.NameClaimType
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(
                token, validationParameters, out _);

            return principal;
        }
        catch
        {
            return null;
        }
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
}