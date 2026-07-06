using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Search;
using MediatR;

namespace FlowBoard.Application.Features.Search.Queries.SearchUsers;

public class SearchUsersQueryHandler
    : IRequestHandler<SearchUsersQuery, List<UserSearchDto>>
{
    private const int UserLimit = 10;
    private const int MinQueryLength = 3;

    private readonly ISearchRepository _searchRepository;
    private readonly ICurrentUserService _currentUserService;

    public SearchUsersQueryHandler(
        ISearchRepository searchRepository,
        ICurrentUserService currentUserService)
    {
        _searchRepository = searchRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserSearchDto>> Handle(
        SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query?.Trim() ?? string.Empty;

        if (query.Length < MinQueryLength)
        {
            return [];
        }

        var userId = _currentUserService.GetId();

        return await _searchRepository.SearchUsersAsync(
            userId, query, UserLimit);
    }
}