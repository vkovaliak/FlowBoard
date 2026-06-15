using FlowBoard.Domain.DTOs.Attachments;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadCardAttachment;

public record UploadCardAttachmentCommand(
    Guid CardId,
    Stream FileStream,
    string FileName)
    : IRequest<Result<AttachmentResponse>>;