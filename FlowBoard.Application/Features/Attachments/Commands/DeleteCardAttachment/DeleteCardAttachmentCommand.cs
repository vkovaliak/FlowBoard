using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.DeleteCardAttachment;

public record DeleteCardAttachmentCommand(
    Guid AttachmentId) 
    : IRequest<Result<bool>>;