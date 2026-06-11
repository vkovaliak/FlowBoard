using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Comments.Commands.CreateComment;

public record CreateCommentCommand(
    Guid CardId, 
    string Message) 
    : IRequest<Result<Guid>>;