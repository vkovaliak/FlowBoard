using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.UpdateAvatar;

public record UpdateAvatarCommand(
    Stream FileStream,
    string FileName)
    : IRequest<Result<string>>;