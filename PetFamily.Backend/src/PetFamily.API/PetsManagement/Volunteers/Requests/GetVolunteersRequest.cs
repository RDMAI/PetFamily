using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record GetVolunteersRequest(
    int CurrentPage,
    int PageSize)
{
    public GetVolunteersQuery ToQuery()
    {
        return new GetVolunteersQuery(CurrentPage, PageSize);
    }
}
