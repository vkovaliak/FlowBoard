namespace FlowBoard.Domain.DTOs.Auth;

public record TokenDto(
    string AccessToken, 
    string RefreshToken,
    DateTime AccessTokenExpirationTime,
    DateTime RefreshTokenExpirationTime);