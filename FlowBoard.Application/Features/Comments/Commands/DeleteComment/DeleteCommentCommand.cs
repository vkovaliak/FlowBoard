using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(
    Guid BoardId,
    Guid CardId, 
    Guid CommentId) 
    : IRequest<Result<bool>>;