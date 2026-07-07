namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveListDto
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Name { get; set; } = default!;
    public int Position { get; set; }
    public Guid CreatedBy { get; set; }

    public List<ArchiveCardDto> Cards { get; set; } = [];
}