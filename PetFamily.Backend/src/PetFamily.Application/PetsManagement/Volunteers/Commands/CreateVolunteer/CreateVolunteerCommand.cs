using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
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
