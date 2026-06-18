using FluentValidation;

namespace FlowBoard.Application.Features.Users.Commands.UpdateUserName;

public class UpdateUserNameCommandValidator 
    : AbstractValidator<UpdateUserNameCommand>
{
    public UpdateUserNameCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100);
    }
}