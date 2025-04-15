using FluentValidation;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;

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
