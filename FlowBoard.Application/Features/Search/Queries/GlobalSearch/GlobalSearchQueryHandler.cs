using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Search;
using MediatR;

namespace FlowBoard.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQueryHandler
    : IRequestHandler<GlobalSearchQuery, SearchResultDto>
{
    private const int BoardLimit = 10;
    private const int CardLimit = 20;
    private const int MinQueryLength = 2;

    private readonly ISearchRepository _searchRepository;
    private readonly ICurrentUserService _currentUserService;

    public GlobalSearchQueryHandler(
        ISearchRepository searchRepository,
        ICurrentUserService currentUserService)
    {
        _searchRepository = searchRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SearchResultDto> Handle(
        GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query?.Trim() ?? string.Empty;
        if (query.Length < MinQueryLength)
        {
            return new SearchResultDto([], []);
        }

        var userId = _currentUserService.GetId();

        var boards = await _searchRepository.SearchBoardsAsync(
            userId, query, BoardLimit);

        var cards = await _searchRepository.SearchCardsAsync(
            userId, query, CardLimit);

        return new SearchResultDto(boards, cards);
    }
}