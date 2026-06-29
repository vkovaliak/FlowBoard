using MediatR;
using FlowBoard.Application.Abstractions;
using FluentResults;
using FlowBoard.Domain.Constants;

namespace FlowBoard.Application.Features.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<bool>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommentCommandHandler(
        IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();
        using var uow = _uowFactory.Create();
        try
        {
            var card = await uow.CardRepository.GetByIdAsync(request.CardId);
            if (card == null)
            {
                return Result.Fail(ErrorMessages.CardNotFound); 
            }

            var comment = await uow.CommentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                return Result.Fail($"Comment not found."); 
            }

            if (comment.CreatedBy != currentUserId)
            {
                return Result.Fail("You do not have permission to update this comment.");
            }

            comment.Message = request.Message;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.UpdatedBy = currentUserId;
            
            await uow.CommentRepository.UpdateAsync(comment);

            uow.Commit();

            return true;
        }
        catch
        {
            uow.Rollback();
            return Result.Fail("An error occurred while updating the comment");
        }
        
    }
}