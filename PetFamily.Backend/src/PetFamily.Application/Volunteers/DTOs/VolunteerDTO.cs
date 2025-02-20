namespace PetFamily.Application.Volunteers.DTOs;

public record VolunteerDTO(string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    double ExperienceYears,
    string Phone)
{

}
