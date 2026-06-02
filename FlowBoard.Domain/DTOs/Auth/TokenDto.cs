namespace FlowBoard.Domain.DTOs.Auth;

public record TokenDto(
    string AccessToken, 
    string RefreshToken);