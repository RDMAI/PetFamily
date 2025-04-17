using PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;

namespace PetFamily.PetsManagement.Contracts.Requests.Volunteers;

public record GetVolunteerByIdRequest(Guid Id)
{
    public GetVolunteerByIdQuery ToQuery()
    {
        return new GetVolunteerByIdQuery(Id);
    }
}
