namespace FlowBoard.Domain.DTOs.Labels;

public record LabelDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
}