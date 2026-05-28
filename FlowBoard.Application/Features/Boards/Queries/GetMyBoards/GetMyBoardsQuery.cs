using FlowBoard.Domain.DTOs.Boards;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetMyBoards;

public record GetMyBoardsQuery(Guid CurrentUserId) : IRequest<IEnumerable<BoardDto>>;