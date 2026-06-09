using FluentValidation;

namespace FlowBoard.Application.Features.Cards.Commands.DeleteCard;

public class DeleteCardCommandValidator : AbstractValidator<DeleteCardCommand>
{
    public DeleteCardCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty().WithMessage("Card ID is required.");

        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");
        
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("ListId is reqired");
    }
}