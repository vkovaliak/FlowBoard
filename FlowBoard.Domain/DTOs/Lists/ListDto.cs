using FlowBoard.Domain.DTOs.Cards;

namespace FlowBoard.Domain.DTOs.Lists;

public class ListDto
{
    public Guid Id { get; set; }

    public Guid BoardId { get; set; }

    public string Name { get; set; } = default!;

    public int Position { get; set; }

    public List<CardDto> Cards { get; set; } = [];
}