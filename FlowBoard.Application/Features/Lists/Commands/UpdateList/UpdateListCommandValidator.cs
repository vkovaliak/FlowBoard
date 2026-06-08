using FluentValidation;

namespace FlowBoard.Application.Features.Lists.Commands.UpdateList;

public class UpdateListCommandValidator : AbstractValidator<UpdateListCommand>
{
    public UpdateListCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("List ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("List name cannot be empty.")
            .MaximumLength(100).WithMessage("List name must not exceed 100 characters.");
    }
}