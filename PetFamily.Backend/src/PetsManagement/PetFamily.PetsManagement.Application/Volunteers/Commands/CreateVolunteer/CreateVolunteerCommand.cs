using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer;
public record CreateVolunteerCommand(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone) : ICommand;
