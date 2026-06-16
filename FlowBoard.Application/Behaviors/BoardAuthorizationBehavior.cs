using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Behaviors;

public class BoardAuthorizationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly IBoardRepository _boardRepository;
    private readonly ICurrentUserService _currentUserService;

    public BoardAuthorizationBehavior(
        IBoardRepository boardRepository, ICurrentUserService currentUserService)
    {
        _boardRepository = boardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        if (requestName.EndsWith("Query"))
        {
            return await next(cancellationToken);
        }

        Guid? boardId = null;

        var boardIdProp = typeof(TRequest).GetProperty("BoardId");
        if (boardIdProp?.GetValue(request) is Guid bId)
        {
            boardId = bId;
        }
        else if (typeof(TRequest).GetProperty("CardId")?.GetValue(
            request) is Guid cardId)
        {
            boardId = await _boardRepository.GetBoardIdByCardIdAsync(cardId);
        }
        else if (typeof(TRequest).GetProperty("CommentId")?.GetValue(
            request) is Guid commentId)
        {
            boardId = await _boardRepository.GetBoardIdByCommentIdAsync(commentId);
        }
        else if (typeof(TRequest).GetProperty("AttachmentId")?.GetValue(
            request) is Guid attachmentId)
        {
            var commandName = typeof(TRequest).Name;
            if (commandName.Contains("CardAttachment"))
            {
                boardId = await _boardRepository.GetBoardIdByCardAttachmentIdAsync(
                    attachmentId);
            }
            else if (commandName.Contains("CommentAttachment"))
            {
                boardId = await _boardRepository.GetBoardIdByCommentAttachmentIdAsync(
                    attachmentId);
            }
        }

        if (boardId.HasValue)
        {
            var currentUserId = _currentUserService.GetId();
            var userRole = await _boardRepository.GetUserRoleAsync(
                boardId.Value, currentUserId);

            if (userRole is null || userRole == BoardRole.Viewer)
            {
                var result = new TResponse();
                result.Reasons.Add(
                    new Error("You do not have permission to modify data on this board."));
                return result;
            }
        }

        return await next(cancellationToken);
    }
}