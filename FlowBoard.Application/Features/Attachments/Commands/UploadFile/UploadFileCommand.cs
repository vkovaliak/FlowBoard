using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Attachments.Commands.UploadFile;

public record UploadFileCommand(
    Stream FileStream, 
    string FileName) 
    : IRequest<Result<string>>;