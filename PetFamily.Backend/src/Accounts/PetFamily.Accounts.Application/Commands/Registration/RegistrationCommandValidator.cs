using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.Registration;

public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("UserName"));

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Password"));

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Email"));
    }
}
