using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Boards;

public class BoardAccessDto
{
    public Guid BoardId { get; set; }
    public bool IsPublic { get; set; }
    public bool IsMember => MemberRole.HasValue;
    public BoardRole? MemberRole { get; set; }
}