namespace FlowBoard.Domain.DTOs.Search;

public record UserSearchDto(
    Guid Id,
    string EmailAddress,
    string UserName,
    string? AvatarUrl);