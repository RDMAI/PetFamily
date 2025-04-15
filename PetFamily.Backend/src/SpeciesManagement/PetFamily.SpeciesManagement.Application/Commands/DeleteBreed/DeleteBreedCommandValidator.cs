using FluentValidation;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;

namespace PetFamily.SpeciesManagement.Application.Commands.DeleteBreed;

public class DeleteBreedCommandValidator : AbstractValidator<DeleteBreedCommand>
{
    public DeleteBreedCommandValidator()
    {
        RuleFor(c => c.SpeciesId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("SpeciesId"));
        RuleFor(c => c.BreedId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("BreedId"));
    }
}
