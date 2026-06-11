using FluentResults;
using MediatR;
using FlowBoard.Domain.DTOs.Comments;

namespace FlowBoard.Application.Features.Comments.Queries.GetCommentsByCardId;

public record GetCommentsByCardIdQuery(
    Guid CardId) :
     IRequest<Result<IEnumerable<CommentDto>>>;