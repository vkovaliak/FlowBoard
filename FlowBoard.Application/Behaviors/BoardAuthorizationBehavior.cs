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
        if (typeof(TRequest).Name.EndsWith("Query"))
        {
            return await next();
        }

        var boardIdProp = typeof(TRequest).GetProperty("BoardId");
        
        if (boardIdProp?.GetValue(request) is Guid boardId)
        {
            var currentUserId = _currentUserService.GetId();
            var userRole = await _boardRepository.GetUserRoleAsync(
                boardId, currentUserId);

            if (userRole is null || userRole == BoardRole.Viewer)
            {
                var result = new TResponse();
                result.Reasons.Add(
                    new Error("You do not have permission to modify data on this board."));
                return result;
            }
        }

        return await next();
    }
}