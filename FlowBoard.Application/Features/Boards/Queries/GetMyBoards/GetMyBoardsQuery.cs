using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetMyBoards;

public record GetMyBoardsQuery(
    ArchiveStatus Status)
    : IRequest<Result<IEnumerable<BoardDto>>>;