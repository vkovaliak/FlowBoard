using System.Security.Claims;
using FlowBoard.Domain.DTOs.Auth;

namespace FlowBoard.Application.Abstractions;

public interface IExternalAuthService
{
    ExternalUserInfo ExtractUserInfo(ClaimsPrincipal principal);
    string BuildCallbackUrl(TokenDto token);
    string BuildErrorUrl(string error);
}