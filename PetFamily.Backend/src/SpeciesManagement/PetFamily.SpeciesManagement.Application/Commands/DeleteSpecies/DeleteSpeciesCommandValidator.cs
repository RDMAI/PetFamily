using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

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
