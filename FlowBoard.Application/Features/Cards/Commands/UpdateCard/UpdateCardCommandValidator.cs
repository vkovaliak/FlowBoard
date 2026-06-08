using FluentValidation;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCard;

public class UpdateCardCommandValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("List ID is required.");

        RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("Card ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Card name cannot be empty.")
            .MaximumLength(100).WithMessage("Card name must not exceed 100 characters.");
    }
}