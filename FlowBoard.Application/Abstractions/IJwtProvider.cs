namespace FlowBoard.Application.Abstractions;

public interface IJwtProvider
{
    (string, DateTime) GenerateAccessToken(Guid userId, string email);
    (string, DateTime) GenerateRefreshToken();
}