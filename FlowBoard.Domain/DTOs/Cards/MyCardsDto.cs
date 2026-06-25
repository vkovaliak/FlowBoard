using FlowBoard.Domain.DTOs.CardAssignee;
using FlowBoard.Domain.DTOs.Labels;

namespace FlowBoard.Domain.DTOs.Cards;

public class MyCardDto
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public Guid BoardId { get; set; }
    public string BoardName { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public List<LabelDto> Labels { get; set; } = [];
    public List<CardAssigneeDto> Assignees { get; set; } = [];
}