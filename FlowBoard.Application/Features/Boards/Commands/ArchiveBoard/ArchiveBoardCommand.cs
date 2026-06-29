using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ArchiveBoard;

public record ArchiveBoardCommand(
    Guid BoardId) 
    : IRequest<Result<bool>>;