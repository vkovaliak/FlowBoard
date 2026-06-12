using FluentValidation;

namespace FlowBoard.Application.Features.Comments.Commands.UpdateComment;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("CardId is required.");

        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("CardId is required.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Comment message cannot be empty.");
    }
}