using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.DeleteCardAttachment;

public record DeleteCardAttachmentCommand(
    Guid BoardId,
    Guid AttachmentId) 
    : IRequest<Result<bool>>;