using FlowBoard.Domain.DTOs.Search;

namespace FlowBoard.Application.Abstractions;

public interface ISearchRepository
{
    Task<List<SearchBoardDto>> SearchBoardsAsync(
        Guid userId, string query, int limit);

    Task<List<SearchCardDto>> SearchCardsAsync(
        Guid userId, string query, int limit);

    Task<List<UserSearchDto>> SearchUsersAsync(
        Guid currentUserId, string query, int limit);
}