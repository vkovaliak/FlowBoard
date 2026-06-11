using MediatR;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using FluentResults;

namespace FlowBoard.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ICurrentUserService _currentUserService;

    public CreateCommentCommandHandler(IUnitOfWorkFactory uowFactory, ICurrentUserService currentUserService)
    {
        _uowFactory = uowFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
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

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                CardId = request.CardId,
                Message = request.Message,
                CreatedBy = currentUserId
            };

            
            await uow.CommentRepository.CreateAsync(comment);

            uow.Commit();

            return comment.Id;
        }
        catch
        {
            uow.Rollback();
            return Result.Fail("An error occurred while creating the comment");
        }
        
    }
}