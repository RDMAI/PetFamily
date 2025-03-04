namespace PetFamily.Application.PetsManagement.Volunteers.UpdateMainInfo;
public record UpdateMainInfoCommand(
    Guid VolunteerId,
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone);
