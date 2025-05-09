using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.RefreshTokens;

public class RefreshTokensCommandValidator : AbstractValidator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(c => c.AccessToken)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("AccessToken"));

        RuleFor(c => c.RefreshToken)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("RefreshToken"));
    }
}
