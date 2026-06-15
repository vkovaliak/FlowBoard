using FluentResults;
using FlowBoard.Application.Abstractions;
using MediatR;
using FlowBoard.Domain.DTOs.Attachments;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Constants;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadCardAttachment;

public class UploadCardAttachmentCommandHandler : IRequestHandler<UploadCardAttachmentCommand, Result<AttachmentResponseDto>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ICardAttachmentRepository _cardAttachmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public UploadCardAttachmentCommandHandler(
        IFileStorageService fileStorageService,
        ICardAttachmentRepository cardAttachmentRepository,
        ICurrentUserService currentUserService)
    {
        _fileStorageService = fileStorageService;
        _cardAttachmentRepository = cardAttachmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<AttachmentResponseDto>> Handle(
        UploadCardAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.GetId();
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

            await _cardAttachmentRepository.CreateAsync(attachment);

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