using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Activities;

public class ActivityDto
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public ActivityAction ActionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}