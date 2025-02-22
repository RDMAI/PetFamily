using CSharpFunctionalExtensions;
using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Application.Volunteers.Commands;
using PetFamily.Application.Volunteers.DTOs;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Volunteers.Validators;
public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(c => new {
            c.VolunteerDTO.FirstName,
            c.VolunteerDTO.LastName,
            c.VolunteerDTO.FatherName
        }).MustBeValueObject(d => VolunteerFullName.Create(
            d.FirstName,
            d.LastName,
            d.FatherName));

        RuleFor(c => c.VolunteerDTO.Email).MustBeValueObject(Email.Create);
        RuleFor(c => c.VolunteerDTO.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.VolunteerDTO.Phone).MustBeValueObject(Phone.Create);
        RuleFor(c => c.VolunteerDTO.ExperienceYears).MustBeValueObject(VolunteerExperienceYears.Create);

        RuleForEach(c => c.RequisitesList).MustBeValueObject(CreateRequisitesFromDTO);
        RuleForEach(c => c.SocialNetworksList).MustBeValueObject(CreateNetworksFromDTO);

        //RuleFor(c => c.RequisitesList).MustBeListOfValueObjects(CreateRequisitesFromDTO);
        //RuleFor(c => c.SocialNetworksList).MustBeListOfValueObjects(CreateNetworksFromDTO);
    }

    private Result<Requisites, Error> CreateRequisitesFromDTO(RequisitesDTO dto)
        => Requisites.Create(dto.Name, dto.Description, dto.Value);
    private Result<SocialNetwork, Error> CreateNetworksFromDTO(SocialNetworkDTO dto)
        => SocialNetwork.Create(dto.Name, dto.Link);
}
