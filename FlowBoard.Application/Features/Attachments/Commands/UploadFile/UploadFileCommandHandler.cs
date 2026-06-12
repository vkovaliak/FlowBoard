using FluentResults;
using FlowBoard.Application.Abstractions;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadFile;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<string>>
{
    private readonly IFileStorageService _fileStorageService;

    public UploadFileCommandHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<string>> Handle(
        UploadFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            string fileUrl = await _fileStorageService.UploadAsync(
                request.FileStream, 
                request.FileName, 
                "attachments"); 

            return Result.Ok(fileUrl);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.ToString());
        }
    }
}