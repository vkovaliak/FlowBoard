using FluentValidation;

namespace FlowBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandValidator 
    : AbstractValidator<UpdateChecklistItemCommand>
{
    public UpdateChecklistItemCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Checklist item text is required")
            .MaximumLength(500);
    }
}