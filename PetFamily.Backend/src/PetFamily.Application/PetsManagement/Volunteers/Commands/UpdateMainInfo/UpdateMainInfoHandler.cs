using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateMainInfo
{
    public class UpdateMainInfoHandler
        : ICommandHandler<VolunteerId, UpdateMainInfoCommand>
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly UpdateMainInfoCommandValidator _validator;
        private readonly ILogger<UpdateMainInfoHandler> _logger;

        public UpdateMainInfoHandler(
            IVolunteerRepository volunteerRepository,
            UpdateMainInfoCommandValidator validator,
            ILogger<UpdateMainInfoHandler> logger)
        {
            _volunteerRepository = volunteerRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<VolunteerId, ErrorList>> HandleAsync(
            UpdateMainInfoCommand command,
            CancellationToken cancellationToken = default)
        {
            // command validation
            var validatorResult = await _validator.ValidateAsync(command, cancellationToken);

            if (!validatorResult.IsValid)
            {
                var errors = from e in validatorResult.Errors
                             select Error.Deserialize(e.ErrorMessage);
                return new ErrorList(errors);
            }

            // check if volunteer exist
            var volunteerId = VolunteerId.Create(command.VolunteerId);
            var entityResult = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (entityResult.IsFailure)
                return entityResult.Error;

            // create VOs
            var fullName = VolunteerFullName.Create(command.FirstName,
                command.LastName,
                command.FatherName).Value;
            var email = Email.Create(command.Email).Value;
            var description = Description.Create(command.Description).Value;
            var phone = Phone.Create(command.Phone).Value;
            var experienceYears = VolunteerExperienceYears.Create(command.ExperienceYears).Value;

            // check if Entity with this phone or email already exists
            var emailResponse = await _volunteerRepository.IsEmailNotExistAsync(email, cancellationToken);
            if (emailResponse.IsFailure)
                return emailResponse.Error;
            var phoneResponse = await _volunteerRepository.IsPhoneNotExistAsync(phone, cancellationToken);
            if (phoneResponse.IsFailure)
                return phoneResponse.Error;

            // update entity
            var entity = entityResult.Value;
            entity.UpdateMainInfo(fullName,
                email,
                description,
                experienceYears,
                phone);

            // handle BL
            var response = await _volunteerRepository.UpdateAsync(entity, cancellationToken);
            //if (createResponse.IsFailure) return createResponse.Error;

            _logger.LogInformation("Main info for volunteer with id {Id} was updated", volunteerId.Value);

            return response;
        }
    }
}
