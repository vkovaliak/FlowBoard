using FluentValidation;

namespace FlowBoard.Application.Features.Lists.Commands.MoveList;

public class MoveListCommandValidator : AbstractValidator<MoveListCommand>
{
    public MoveListCommandValidator()
    {
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("Board ID is required.");;

        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("List ID is required.");;

        RuleFor(x => x.NewPosition).GreaterThanOrEqualTo(0)
            .WithMessage("Position cannot be negative.");
    }
}