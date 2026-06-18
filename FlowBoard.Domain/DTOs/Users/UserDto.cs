namespace FlowBoard.Domain.DTOs.Users;

public record UserDto(
    Guid Id,
    string EmailAddress,
    string UserName,
    string? AvatarUrl);