namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveLabelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public Guid CreatedBy { get; set; }
}