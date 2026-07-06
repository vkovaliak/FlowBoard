using FluentResults;
using MediatR;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.DTOs.Boards;

namespace FlowBoard.Application.Features.Boards.Queries.GetBackgrounds;

public class GetBackgroundsQueryHandler
    : IRequestHandler<GetBackgroundsQuery, Result<List<BoardBackgroundDto>>>
{
    private readonly IFileStorageService _fileStorageService;

    public GetBackgroundsQueryHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<BoardBackgroundDto>>> Handle(
        GetBackgroundsQuery request, CancellationToken cancellationToken)
    {
        var urls = await _fileStorageService.ListBlobUrlsAsync(
            StorageConstants.BoardBackgroundsContainer);

        var result = urls
            .Select(u => new BoardBackgroundDto(u))
            .ToList();

        return Result.Ok(result);
    }
}