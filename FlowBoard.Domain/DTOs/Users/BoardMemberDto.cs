using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Users;

public class BoardMemberDto
{
    public Guid UserId { get; set; }
    public string EmailAddress { get; set; } = default!;
    public BoardRole Role { get; set; }
    public required string UserName { get; set; }
    public string? AvatarUrl { get; set; }
}