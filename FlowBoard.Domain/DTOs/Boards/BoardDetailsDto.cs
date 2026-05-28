using FlowBoard.Domain.DTOs.Lists;

namespace FlowBoard.Domain.DTOs.Boards;

public class BoardDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public bool IsPublic { get; set; }

    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public List<ListDto> Lists { get; set; } = [];
}