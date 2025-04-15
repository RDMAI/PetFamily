using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
public class UpdateRequisitesCommandValidator : AbstractValidator<UpdateRequisitesCommand>
{
    public UpdateRequisitesCommandValidator()
    {
        RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        RuleFor(c => c.RequisitesList)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("Requisites"));

        RuleForEach(c => c.RequisitesList).MustBeValueObject(CreateRequisitesFromDTO);
    }

    private Result<Requisites, Error> CreateRequisitesFromDTO(RequisitesDTO dto)
        => Requisites.Create(dto.Name, dto.Description, dto.Value);
}
