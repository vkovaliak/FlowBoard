using FluentValidation;

namespace FlowBoard.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("CardId is required.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Comment message cannot be empty.");
    }
}