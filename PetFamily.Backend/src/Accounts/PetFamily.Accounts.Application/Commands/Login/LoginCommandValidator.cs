using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("UserName"));

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Password"));
    }
}
