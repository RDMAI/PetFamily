using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.SpeciesManagement.Application.Commands.DeleteSpecies;

public class DeleteSpeciesCommandValidator : AbstractValidator<DeleteSpeciesCommand>
{
    public DeleteSpeciesCommandValidator()
    {
        RuleFor(c => c.SpeciesId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("SpeciesId"));
    }
}
