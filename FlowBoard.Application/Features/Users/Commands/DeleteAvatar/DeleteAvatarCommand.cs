using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.DeleteAvatar;

public record DeleteAvatarCommand()
    : IRequest<Result<bool>>;