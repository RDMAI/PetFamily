using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(c => new
        {
            c.FirstName,
            c.LastName,
            c.FatherName
        }).MustBeValueObject(d => VolunteerFullName.Create(
            d.FirstName,
            d.LastName,
            d.FatherName));

        RuleFor(c => c.Email).MustBeValueObject(Email.Create);
        RuleFor(c => c.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.ExperienceYears).MustBeValueObject(VolunteerExperienceYears.Create);

        RuleFor(c => c.RequisitesList).MustBeNotNull();
        RuleForEach(c => c.RequisitesList).MustBeValueObject(CreateRequisitesFromDTO);
        RuleFor(c => c.SocialNetworksList).MustBeNotNull();
        RuleForEach(c => c.SocialNetworksList).MustBeValueObject(CreateNetworksFromDTO);
    }

    private Result<Requisites, Error> CreateRequisitesFromDTO(RequisitesDTO dto)
        => Requisites.Create(dto.Name, dto.Description, dto.Value);
    private Result<SocialNetwork, Error> CreateNetworksFromDTO(SocialNetworkDTO dto)
        => SocialNetwork.Create(dto.Name, dto.Link);
}
