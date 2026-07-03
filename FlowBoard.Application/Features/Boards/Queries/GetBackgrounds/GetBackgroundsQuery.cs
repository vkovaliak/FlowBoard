using FluentResults;
using MediatR;
using FlowBoard.Domain.DTOs.Boards;

namespace FlowBoard.Application.Features.Boards.Queries.GetBackgrounds;

public record GetBackgroundsQuery 
    : IRequest<Result<List<BoardBackgroundDto>>>;