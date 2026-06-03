using FlowBoard.Domain.DTOs.Boards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetMyBoards;

public record GetMyBoardsQuery(
    Guid CurrentUserId) 
    : IRequest<Result<IEnumerable<BoardDto>>>;