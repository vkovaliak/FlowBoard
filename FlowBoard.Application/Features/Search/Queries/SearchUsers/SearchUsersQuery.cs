using FlowBoard.Domain.DTOs.Search;
using MediatR;

namespace FlowBoard.Application.Features.Search.Queries.SearchUsers;

public record SearchUsersQuery(string Query)
    : IRequest<List<UserSearchDto>>;