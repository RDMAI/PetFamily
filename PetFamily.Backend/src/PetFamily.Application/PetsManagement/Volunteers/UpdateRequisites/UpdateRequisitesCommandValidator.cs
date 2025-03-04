using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
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
