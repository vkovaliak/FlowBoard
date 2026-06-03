using FluentValidation;

namespace FlowBoard.Application.Features.Lists.Commands.DeleteList;

public class DeleteListCommandValidator : AbstractValidator<DeleteListCommand>
{
    public DeleteListCommandValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("List ID is required.");

        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");
    }
}