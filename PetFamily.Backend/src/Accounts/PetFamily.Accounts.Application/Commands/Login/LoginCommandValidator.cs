using FluentValidation;

namespace PetFamily.Accounts.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
    }
}
