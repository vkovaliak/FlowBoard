using FluentResults;
using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.DeleteCardAttachment;

public class DeleteCardAttachmentCommandHandler 
    : IRequestHandler<DeleteCardAttachmentCommand, Result<bool>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ICardAttachmentRepository _cardAttachmentRepository;

    public DeleteCardAttachmentCommandHandler(
        IFileStorageService fileStorageService,
        ICardAttachmentRepository cardAttachmentRepository)
    {
        _fileStorageService = fileStorageService;
        _cardAttachmentRepository = cardAttachmentRepository;
    }

    public async Task<Result<bool>> Handle(
        DeleteCardAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var attachment = await _cardAttachmentRepository.GetByIdAsync(request.AttachmentId);
            if (attachment is null)
            {
                return Result.Fail("Attachment not found.");
            }

            await _fileStorageService.DeleteAsync(attachment.BlobUrl);

            await _cardAttachmentRepository.DeleteAsync(attachment);

            return Result.Ok(true);
        }
        catch
        {
            return Result.Fail("An error occurred while deleting the card attachment");
        }
    }
}