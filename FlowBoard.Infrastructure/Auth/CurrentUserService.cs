using System.Security.Claims;
using FlowBoard.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FlowBoard.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null)
        {
            return Guid.Empty;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? user.FindFirst("sub")?.Value;

        return Guid.TryParse(userIdClaim, out var parsedGuid) ? parsedGuid : Guid.Empty;
    }
}