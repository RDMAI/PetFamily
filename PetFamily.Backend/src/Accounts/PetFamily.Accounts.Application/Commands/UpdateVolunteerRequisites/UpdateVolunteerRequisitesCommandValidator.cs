using FluentValidation;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Core.Validation;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;

public class UpdateVolunteerRequisitesCommandValidator : AbstractValidator<UpdateVolunteerRequisitesCommand>
{
    public UpdateVolunteerRequisitesCommandValidator()
    {
        RuleFor(c => c.Requisites)
            .MustBeNotNull();

        RuleForEach(c => c.Requisites).MustBeValueObject((RequisitesDTO dto) =>
            Requisites.Create(dto.Name, dto.Description, dto.Value));

        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("UserId"));
    }
}
