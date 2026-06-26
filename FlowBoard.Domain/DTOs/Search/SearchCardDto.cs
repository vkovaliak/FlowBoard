namespace FlowBoard.Domain.DTOs.Search;

public record SearchCardDto(
    Guid Id,
    string Name,
    Guid BoardId,
    string BoardName);