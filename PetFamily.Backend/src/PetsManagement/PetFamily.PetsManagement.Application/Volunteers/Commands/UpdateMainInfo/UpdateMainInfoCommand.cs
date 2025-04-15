using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateMainInfo;
public record UpdateMainInfoCommand(
    Guid VolunteerId,
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone) : ICommand;
