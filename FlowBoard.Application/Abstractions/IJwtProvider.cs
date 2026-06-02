namespace FlowBoard.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateAccessToken(Guid userId, string email);
    string GenerateRefreshToken();
}