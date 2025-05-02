using FluentValidation;

namespace PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;

public class UpdateUserMainInfoCommandValidator : AbstractValidator<UpdateUserMainInfoCommand>
{
    public UpdateUserMainInfoCommandValidator()
    {
    }
}
