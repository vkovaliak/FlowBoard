using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.DeleteCommentAttachment;

public record DeleteCommentAttachmentCommand(
    Guid BoardId,
    Guid AttachmentId) 
    : IRequest<Result<bool>>;