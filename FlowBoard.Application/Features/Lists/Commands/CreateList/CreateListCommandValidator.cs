using FluentValidation;

namespace FlowBoard.Application.Features.Lists.Commands.CreateList;

public class CreateListCommandValidator : AbstractValidator<CreateListCommand>
{
    public CreateListCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("List name is required.")
            .MaximumLength(10).WithMessage("List name must not exceed 100 characters.");
    }
}