using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer;
public record CreateVolunteerCommand(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone,
    IEnumerable<RequisitesDTO> RequisitesList,
    IEnumerable<SocialNetworkDTO> SocialNetworksList) : ICommand;
