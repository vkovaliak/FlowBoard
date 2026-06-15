using FluentResults;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using MediatR;
using FlowBoard.Domain.DTOs.Attachments;
using FlowBoard.Domain.Constants;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadCommentAttachment;

public class UploadCommentAttachmentCommandHandler : IRequestHandler<UploadCommentAttachmentCommand, Result<AttachmentResponseDto>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommentAttachmentRepository _commentAttachmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public UploadCommentAttachmentCommandHandler(
        IFileStorageService fileStorageService,
        ICommentAttachmentRepository commentAttachmentRepository,
        ICurrentUserService currentUserService)
    {
        _fileStorageService = fileStorageService;
        _commentAttachmentRepository = commentAttachmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<AttachmentResponseDto>> Handle(
        UploadCommentAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.GetId();
            string fileUrl = await _fileStorageService.UploadAsync(
                request.FileStream,
                request.FileName,
                StorageConstants.CommentAttachmentsContainer);

            var attachment = new CommentAttachment
            {
                Id = Guid.NewGuid(),
                CommentId = request.CommentId,
                FileName = request.FileName,
                BlobUrl = fileUrl,
                ContentType = Path.GetExtension(request.FileName).ToLowerInvariant(),
                UploadetBy = currentUserId
            };

            await _commentAttachmentRepository.CreateAsync(attachment);

            return Result.Ok(new AttachmentResponseDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                BlobUrl = attachment.BlobUrl
            });
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.ToString());
        }
    }
}