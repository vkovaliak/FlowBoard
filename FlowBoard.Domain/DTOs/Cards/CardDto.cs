namespace FlowBoard.Domain.DTOs.Cards;

public class CardDto
{
    public Guid Id { get; set; }

    public Guid ListId { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public int Position { get; set; }
}