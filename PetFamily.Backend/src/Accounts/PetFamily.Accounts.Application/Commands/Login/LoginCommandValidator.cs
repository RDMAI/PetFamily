using FluentValidation;
using PetFamily.Accounts.Application.Commands.Login;

namespace PetFamily.Accounts.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
    }
}
