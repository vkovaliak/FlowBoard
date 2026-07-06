namespace FlowBoard.Domain.DTOs.Boards;

public record BoardMemberAvatarDto(
    Guid UserId,
    string UserName,
    string? AvatarUrl);