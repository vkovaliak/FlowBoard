using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlowBoard.Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    protected readonly JwtOptions _options;
    public JwtProvider(IOptions<JwtOptions> jwtOptions)
    {
        _options = jwtOptions.Value;
    }

    public string GenerateAccessToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber);
    }
}