using FlowBoard.Domain.DTOs.Boards;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardDetails;

public record GetBoardDetailsQuery(Guid BoardId) : IRequest<BoardDetailsDto?>;