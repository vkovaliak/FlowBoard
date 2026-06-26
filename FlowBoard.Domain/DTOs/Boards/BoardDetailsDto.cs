using FlowBoard.Domain.DTOs.Lists;
using FlowBoard.Domain.DTOs.Users;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Boards;

public class BoardDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public bool IsPublic { get; set; }

    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public bool IsFavorite { get; set; }

    public BoardRole? UserRole { get; set; }
    
    public List<ListDto> Lists { get; set; } = [];

    public List<BoardMemberDto> Members { get; set; } = [];
}