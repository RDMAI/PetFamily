using FluentValidation;

namespace PetFamily.Accounts.Application.Commands.Registration;

public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationCommandValidator()
    {
    }
}
