namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record UpdateMainInfoRequest(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone);