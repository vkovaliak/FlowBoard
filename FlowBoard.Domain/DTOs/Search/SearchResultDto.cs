namespace FlowBoard.Domain.DTOs.Search;

public record SearchResultDto(
    List<SearchBoardDto> Boards,
    List<SearchCardDto> Cards);