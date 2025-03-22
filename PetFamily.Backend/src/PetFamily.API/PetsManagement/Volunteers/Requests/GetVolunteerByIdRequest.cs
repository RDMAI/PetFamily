using PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record GetVolunteerByIdRequest(Guid Id)
{
    public GetVolunteerByIdQuery ToQuery()
    {
        return new GetVolunteerByIdQuery(Id);
    }
}
