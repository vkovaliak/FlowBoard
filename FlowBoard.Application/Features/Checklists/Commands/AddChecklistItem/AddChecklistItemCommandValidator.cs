using FluentValidation;

namespace FlowBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public class AddChecklistItemCommandValidator 
    : AbstractValidator<AddChecklistItemCommand>
{
    public AddChecklistItemCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Checklist item text is required")
            .MaximumLength(500);
    }
}