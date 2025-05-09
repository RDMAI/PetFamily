using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;

public class UpdateUserMainInfoCommandValidator : AbstractValidator<UpdateUserMainInfoCommand>
{
    public UpdateUserMainInfoCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("FirstName"));

        RuleFor(c => c.LastName)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("LastName"));

        RuleFor(c => c.FatherName)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("FatherName"));

        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("UserId"));
    }
}
