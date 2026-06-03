using FlowBoard.Domain.DTOs.Boards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetMyBoards;

public record GetMyBoardsQuery
    : IRequest<Result<IEnumerable<BoardDto>>>;