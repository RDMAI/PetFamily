using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateMainInfo;

namespace PetFamily.PetsManagement.Contracts.Requests.Volunteers;

public record UpdateMainInfoRequest(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone)
{
    public UpdateMainInfoCommand ToCommand(Guid volunteerId)
    {
        return new UpdateMainInfoCommand(
            volunteerId,
            FirstName,
            LastName,
            FatherName,
            Email,
            Description,
            ExperienceYears,
            Phone);
    }
}