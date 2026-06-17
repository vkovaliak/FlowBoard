using FluentValidation;

namespace FlowBoard.Application.Features.Labels.Commands.CreateLabel;

public class CreateLabelCommandValidator : AbstractValidator<CreateLabelCommand>
{
    public CreateLabelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Label name is required")
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .NotEmpty()
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex (e.g. #FF5630)");
    }
}