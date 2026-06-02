namespace FlowBoard.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateToken(Guid userId, string email);
}