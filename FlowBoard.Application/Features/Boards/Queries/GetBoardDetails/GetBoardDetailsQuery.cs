using FlowBoard.Domain.DTOs.Boards;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;

public record GetBoardDetailsQuery(
    Guid BoardId) 
    : IRequest<Result<BoardDetailsDto>>;