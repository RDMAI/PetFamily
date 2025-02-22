using CSharpFunctionalExtensions;
using PetFamily.Application.Volunteers.Commands;
using PetFamily.Application.Volunteers.DTOs;
using PetFamily.Application.Volunteers.Filters;
using PetFamily.Application.Volunteers.Interfaces;
using PetFamily.Application.Volunteers.Validators;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Volunteers.CreateVolunteer
{
    public class CreateVolunteerHandler
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly CreateVolunteerCommandValidator _validator;
        public CreateVolunteerHandler(IVolunteerRepository volunteerRepository,
            CreateVolunteerCommandValidator validator)
        {
            _volunteerRepository = volunteerRepository;
            _validator = validator;
        }

        public async Task<Result<Guid, Error>> HandleAsync(
            CreateVolunteerCommand command,
            CancellationToken cancellationToken = default)
        {
            // command validation
            var validatorResult = await _validator.ValidateAsync(command, cancellationToken);

            if (!validatorResult.IsValid)
            {
                var error = Error.Validation(validatorResult.Errors[0].ErrorCode,
                    validatorResult.Errors[0].ErrorMessage);

                return error;
            }

            // creating ValueObjects vor Entity
            VolunteerDTO volunteerDto = command.VolunteerDTO;
            
            var fullName = VolunteerFullName.Create(volunteerDto.FirstName,
                volunteerDto.LastName,
                volunteerDto.FatherName).Value;

            var email = Email.Create(volunteerDto.Email).Value;
            var description = Description.Create(volunteerDto.Description).Value;
            var phone = Phone.Create(volunteerDto.Phone).Value;
            var experienceYears = VolunteerExperienceYears.Create(volunteerDto.ExperienceYears).Value;

            List<Requisites> requisitesBufferList = [];
            foreach (RequisitesDTO requisites in command.RequisitesList)
            {
                requisitesBufferList.Add(Requisites.Create(requisites.Name,
                    requisites.Description,
                    requisites.Value).Value);
            }
            var requisitesList = RequisitesList.Create(requisitesBufferList).Value;

            List<SocialNetwork> socialNetworkBufferList = [];
            foreach (SocialNetworkDTO socialNetwork in command.SocialNetworksList)
            {
                socialNetworkBufferList.Add(SocialNetwork.Create(socialNetwork.Name,
                    socialNetwork.Link).Value);
            }
            var socialNetworkList = SocialNetworkList.Create(socialNetworkBufferList).Value;

            var entity = new Volunteer(VolunteerId.GenerateNew(),
                fullName,
                email,
                description,
                experienceYears,
                phone,
                requisitesList,
                socialNetworkList);

            // check if this Entity exists:
            // with the same email
            var emailFilter = new VolunteerFilter(Email: email);
            var emailResponse = await _volunteerRepository.GetAsync(emailFilter, cancellationToken);
            if (emailResponse.IsFailure) return emailResponse.Error;  // in case of DB error
            if (emailResponse.Value.Any()) return ErrorHelper.General.AlreadyExist("Volunteer");
            // with the same email
            var phoneFilter = new VolunteerFilter(Phone: phone);
            var phoneResponse = await _volunteerRepository.GetAsync(phoneFilter, cancellationToken);
            if (phoneResponse.IsFailure) return phoneResponse.Error;  // in case of DB error
            if (phoneResponse.Value.Any()) return ErrorHelper.General.AlreadyExist("Volunteer");

            // handle BL
            var createResponse = await _volunteerRepository.CreateAsync(entity, cancellationToken);
            //if (createResponse.IsFailure) return createResponse.Error;

            return createResponse;
        }
    }
}
