using FlowBoard.Domain.DTOs.Attachments;
using FlowBoard.Domain.DTOs.CardAssignee;

namespace FlowBoard.Domain.DTOs.Cards;

public class CardDto
{
    public Guid Id { get; set; }

    public Guid ListId { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public int Position { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public List<AttachmentResponseDto> Attachments { get; set; } = [];

    public List<CardAssigneeDto> Assignees { get; set; } = [];
}