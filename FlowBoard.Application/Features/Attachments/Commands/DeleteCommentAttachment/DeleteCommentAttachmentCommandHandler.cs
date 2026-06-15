using FluentResults;
using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.DeleteCommentAttachment;

public class DeleteCommentAttachmentCommandHandler 
    : IRequestHandler<DeleteCommentAttachmentCommand, Result<bool>>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommentAttachmentRepository _commentAttachmentRepository;

    public DeleteCommentAttachmentCommandHandler(
        IFileStorageService fileStorageService,
        ICommentAttachmentRepository commentAttachmentRepository)
    {
        _fileStorageService = fileStorageService;
        _commentAttachmentRepository = commentAttachmentRepository;
    }

    public async Task<Result<bool>> Handle(
        DeleteCommentAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var attachment = await _commentAttachmentRepository.GetByIdAsync(request.AttachmentId);
            if (attachment is null)
            {
                return Result.Fail("Comment attachment not found.");
            }

            await _fileStorageService.DeleteAsync(attachment.BlobUrl);

            await _commentAttachmentRepository.DeleteAsync(attachment);

            return Result.Ok(true);
        }
        catch
        {
            return Result.Fail("An error occurred while deleting the comment attachment");
        }
    }
}