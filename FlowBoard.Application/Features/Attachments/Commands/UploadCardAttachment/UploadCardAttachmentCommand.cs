using FlowBoard.Domain.DTOs.Attachments;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadCardAttachment;

public record UploadCardAttachmentCommand(
    Guid BoardId,
    Guid CardId,
    Stream FileStream,
    string FileName)
    : IRequest<Result<AttachmentResponseDto>>;