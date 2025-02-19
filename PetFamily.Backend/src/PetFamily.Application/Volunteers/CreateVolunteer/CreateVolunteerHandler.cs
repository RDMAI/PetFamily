using CSharpFunctionalExtensions;
using PetFamily.Application.Volunteers.Commands;
using PetFamily.Application.Volunteers.DTOs;
using PetFamily.Application.Volunteers.Interfaces;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Volunteers.CreateVolunteer
{
    public class CreateVolunteerHandler
    {
        private readonly IVolunteerRepository _volunteerRepository;
        public CreateVolunteerHandler(IVolunteerRepository volunteerRepository)
        {
            _volunteerRepository = volunteerRepository;
        }

        public async Task<Result<Guid, Error>> HandleAsync(CreateVolunteerCommand command)
        {
            // create and validate entity
            VolunteerDTO volunteerDto = command.VolunteerDTO;
            RequisitesDTO[] requisitesList = command.RequisitesList;
            SocialNetworkDTO[] socialNetworkList = command.SocialNetworksList;
            
            var fullNameResult = VolunteerFullName.Create(volunteerDto.FirstName, volunteerDto.LastName, volunteerDto.FatherName);
            if (fullNameResult.IsFailure) return fullNameResult.Error;

            var emailResult = Email.Create(volunteerDto.Email);
            if (emailResult.IsFailure) return emailResult.Error;

            var phoneResult = Phone.Create(volunteerDto.Phone);
            if (phoneResult.IsFailure) return phoneResult.Error;

            List<Requisites> requisitesBufferList = [];
            foreach (RequisitesDTO requisites in requisitesList)
            {
                var requisitesResult = Requisites.Create(requisites.Name, requisites.Description, requisites.Value);
                if (requisitesResult.IsFailure) return requisitesResult.Error;
                requisitesBufferList.Add(requisitesResult.Value);
            }
            var requisitesListResult = RequisitesList.Create(requisitesBufferList);

            List<SocialNetwork> socialNetworkBufferList = [];
            foreach (SocialNetworkDTO socialNetwork in socialNetworkList)
            {
                var socialNetworkResult = SocialNetwork.Create(socialNetwork.Name, socialNetwork.Link);
                if (socialNetworkResult.IsFailure) return socialNetworkResult.Error;
                socialNetworkBufferList.Add(socialNetworkResult.Value);
            }
            var socialNetworkListResult = SocialNetworkList.Create(socialNetworkBufferList);

            var entityResult = Volunteer.Create(VolunteerId.GenerateNew(),
                fullNameResult.Value,
                emailResult.Value,
                volunteerDto.Description,
                volunteerDto.ExperienceYears,
                phoneResult.Value,
                requisitesListResult.Value,
                socialNetworkListResult.Value
                );
            if (entityResult.IsFailure) return entityResult.Error;

            // handle BL
            var createResponse = await _volunteerRepository.CreateAsync(entityResult.Value);
            //if (createResponse.IsFailure) return createResponse.Error;

            return createResponse;
        }
    }
}
