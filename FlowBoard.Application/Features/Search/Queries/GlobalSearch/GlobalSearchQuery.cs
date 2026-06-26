using FlowBoard.Domain.DTOs.Search;
using MediatR;

namespace FlowBoard.Application.Features.Search.Queries.GlobalSearch;

public record GlobalSearchQuery(
    string Query)
    : IRequest<SearchResultDto>;