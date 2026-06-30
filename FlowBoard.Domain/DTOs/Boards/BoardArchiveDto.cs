using FlowBoard.Domain.DTOs.Lists;
using FlowBoard.Domain.DTOs.Users;

namespace FlowBoard.Domain.DTOs.Boards;

public class BoardArchiveDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsPublic { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
     public List<ListDto> Lists { get; set; } = [];
    public List<BoardMemberDto> Members { get; set; } = [];
}