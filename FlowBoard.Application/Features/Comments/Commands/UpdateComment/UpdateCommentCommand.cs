using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(
    Guid CardId,
    Guid CommentId,
    string Message)
    :IRequest<Result<bool>>;