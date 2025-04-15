using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.PetsManagement.Domain.Entities;
using PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer
{
    public class CreateVolunteerHandler
        : ICommandHandler<VolunteerId, CreateVolunteerCommand>
    {
        private readonly IVolunteerAggregateRepository _volunteerRepository;
        private readonly CreateVolunteerCommandValidator _validator;
        private readonly ILogger<CreateVolunteerHandler> _logger;

        public CreateVolunteerHandler(
            IVolunteerAggregateRepository volunteerRepository,
            CreateVolunteerCommandValidator validator,
            ILogger<CreateVolunteerHandler> logger)
        {
            _volunteerRepository = volunteerRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<VolunteerId, ErrorList>> HandleAsync(
            CreateVolunteerCommand command,
            CancellationToken cancellationToken = default)
        {
            // to test logging
            //_logger.LogInformation("Validation started (test log)");

            // command validation
            var validatorResult = await _validator.ValidateAsync(command, cancellationToken);

            if (!validatorResult.IsValid)
            {
                var errors = from e in validatorResult.Errors
                             select Error.Deserialize(e.ErrorMessage);
                return new ErrorList(errors);
            }

            var fullName = VolunteerFullName.Create(command.FirstName,
                command.LastName,
                command.FatherName).Value;

            var email = Email.Create(command.Email).Value;
            var description = Description.Create(command.Description).Value;
            var phone = Phone.Create(command.Phone).Value;
            var experienceYears = VolunteerExperienceYears.Create(command.ExperienceYears).Value;

            List<Requisites> requisitesBufferList = [];
            foreach (RequisitesDTO requisites in command.RequisitesList)
            {
                requisitesBufferList.Add(Requisites.Create(requisites.Name,
                    requisites.Description,
                    requisites.Value).Value);
            }

            List<SocialNetwork> socialNetworkBufferList = [];
            foreach (SocialNetworkDTO socialNetwork in command.SocialNetworksList)
            {
                socialNetworkBufferList.Add(SocialNetwork.Create(socialNetwork.Name,
                    socialNetwork.Link).Value);
            }

            var volunteerId = VolunteerId.GenerateNew();

            var entity = new Volunteer(volunteerId,
                fullName,
                email,
                description,
                experienceYears,
                phone,
                (ValueObjectList<Requisites>)requisitesBufferList,
                (ValueObjectList<SocialNetwork>)socialNetworkBufferList);

            // check if Entity with this phone or email already exists
            var emailResponse = await _volunteerRepository.IsEmailNotExistAsync(email, cancellationToken);
            if (emailResponse.IsFailure)
                return emailResponse.Error;
            var phoneResponse = await _volunteerRepository.IsPhoneNotExistAsync(phone, cancellationToken);
            if (phoneResponse.IsFailure)
                return phoneResponse.Error;

            // handle BL
            var response = await _volunteerRepository.CreateAsync(entity, cancellationToken);
            //if (createResponse.IsFailure) return createResponse.Error;

            _logger.LogInformation("Volunteer created with {Email} and id {Id}", email.Value, volunteerId.Value);

            return response;
        }
    }
}
