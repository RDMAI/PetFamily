﻿using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer
{
    public class CreateVolunteerHandler
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly CreateVolunteerCommandValidator _validator;
        private readonly ILogger<CreateVolunteerHandler> _logger;

        public CreateVolunteerHandler(
            IVolunteerRepository volunteerRepository,
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
            var requisitesList = RequisitesList.Create(requisitesBufferList).Value;

            List<SocialNetwork> socialNetworkBufferList = [];
            foreach (SocialNetworkDTO socialNetwork in command.SocialNetworksList)
            {
                socialNetworkBufferList.Add(SocialNetwork.Create(socialNetwork.Name,
                    socialNetwork.Link).Value);
            }
            var socialNetworkList = SocialNetworkList.Create(socialNetworkBufferList).Value;

            var volunteerId = VolunteerId.GenerateNew();

            var entity = new Volunteer(volunteerId,
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
            if (emailResponse.Value.Any()) return ErrorHelper.General.AlreadyExist("Volunteer").ToErrorList();
            // with the same email
            var phoneFilter = new VolunteerFilter(Phone: phone);
            var phoneResponse = await _volunteerRepository.GetAsync(phoneFilter, cancellationToken);
            if (phoneResponse.IsFailure) return phoneResponse.Error;  // in case of DB error
            if (phoneResponse.Value.Any()) return ErrorHelper.General.AlreadyExist("Volunteer").ToErrorList();

            // handle BL
            var response = await _volunteerRepository.CreateAsync(entity, cancellationToken);
            //if (createResponse.IsFailure) return createResponse.Error;

            _logger.LogInformation("Volunteer created with {Email} and id {Id}", email.Value, volunteerId.Value);

            return response;
        }
    }
}
