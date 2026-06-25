using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ToggleFavorite;

public record ToggleFavoriteCommand(
    Guid BoardId)
    : IRequest<Result<bool>>;