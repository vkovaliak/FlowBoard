using FlowBoard.Domain.DTOs.Attachments;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadCommentAttachment;

public record UploadCommentAttachmentCommand(
    Guid BoardId,
    Guid CommentId,
    Stream FileStream,
    string FileName)
    : IRequest<Result<AttachmentResponseDto>>;