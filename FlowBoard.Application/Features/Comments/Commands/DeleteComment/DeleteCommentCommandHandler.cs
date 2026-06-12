using FlowBoard.Application.Abstractions;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Comments.Commands.DeleteComment;

public class UpdateCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommentCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var card = await uow.CardRepository.GetByIdAsync(request.CardId);
            if (card == null)
            {
                return Result.Fail($"Card not found."); 
            }

            var comment = await uow.CommentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                return Result.Fail($"Comment not found."); 
            }

            if (comment.CreatedBy != currentUserId)
            {
                return Result.Fail("You do not have permission to delete this comment.");
            }

            var result = await uow.CommentRepository.DeleteAsync(comment);
            
            uow.Commit();

            return result;
        }
        catch
        {
            uow.Rollback();
            return Result.Fail("An error occurred while deleting the comment");
        }
        
    }
}