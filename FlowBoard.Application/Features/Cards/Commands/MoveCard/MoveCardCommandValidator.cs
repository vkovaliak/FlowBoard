using FluentValidation;

namespace FlowBoard.Application.Features.Cards.Commands.MoveCard;

public class MoveCardCommandValidator : AbstractValidator<MoveCardCommand>
{
    public MoveCardCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");

        RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("Card ID is required.");

        RuleFor(x => x.NewListId)
            .NotEmpty().WithMessage("Target List ID is required.");

        RuleFor(x => x.NewPosition)
            .GreaterThanOrEqualTo(0).WithMessage("Position must be 0 or greater.");
    }
}