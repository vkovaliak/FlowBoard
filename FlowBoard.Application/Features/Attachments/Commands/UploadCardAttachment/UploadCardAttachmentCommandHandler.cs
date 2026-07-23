using FluentResults;
using FlowBoard.Application.Abstractions;
using MediatR;
using FlowBoard.Domain.DTOs.Attachments;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadCardAttachment;

public class UploadCardAttachmentCommandHandler : IRequestHandler<UploadCardAttachmentCommand, Result<AttachmentResponseDto>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UploadCardAttachmentCommandHandler(
        IFileStorageService fileStorageService,
        IUnitOfWorkFactory uowFactory,
        ICurrentUserService currentUserService)
    {
        _fileStorageService = fileStorageService;
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<AttachmentResponseDto>> Handle(
        UploadCardAttachmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            string fileUrl = await _fileStorageService.UploadAsync(
                request.FileStream,
                request.FileName,
                StorageConstants.CardAttachmentsContainer);

            var attachment = new CardAttachment
            {
                Id = Guid.NewGuid(),
                CardId = request.CardId,
                FileName = request.FileName,
                BlobUrl = fileUrl,
                ContentType = Path.GetExtension(request.FileName).ToLowerInvariant(),
                UploadetBy = currentUserId
            };

            await uow.CardAttachmentRepository.CreateAsync(attachment);

            var user = await uow.UserRepository.GetByIdAsync(currentUserId);
            if (user is null)
            {
                return Result.Fail("User is not found");
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                CardId = request.CardId, 
                BoardId = request.BoardId,
                UserId = currentUserId,
                ActionType = ActivityAction.CommentAttachmentsAdded,
                Description = $"Attachment '{request.FileName}' added to card by {user.UserName}",
                CreatedAt = DateTime.UtcNow
            };

            await uow.ActivityRepository.CreateAsync(activity);

            uow.Commit();

            return Result.Ok(new AttachmentResponseDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                BlobUrl = attachment.BlobUrl
            });
        }
        catch
        {
            uow.Rollback();
            return Result.Fail("An error occurred while uploading");
        }
    }
}