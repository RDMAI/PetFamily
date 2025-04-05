using PetFamily.Application.PetsManagement.Pets.Queries.GetPets;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Pets.Requests;

public record GetPetsRequest(
    int CurrentPage,
    int PageSize,
    IEnumerable<SortByDTO>? Sort = null,
    string? Name = null,
    string? Color = null,
    float? MinWeight = null,
    float? MaxWeight = null,
    float? MinHeight = null,
    float? MaxHeight = null,
    Guid? Breed_Id = null,
    Guid? Species_Id = null,
    string? Health_Information = null,
    string? City = null,
    string? Street = null,
    string? House_Number = null,
    string? House_SubNumber = null,
    string? Appartment_Number = null,
    string? Owner_Phone = null,
    bool? Is_Castrated = null,
    int? MinAge = null,
    int? MaxAge = null,
    bool? Is_Vacinated = null,
    int? Status = null)
{
    public GetPetsQuery ToQuery(Guid VolunteerId)
    {
        return new GetPetsQuery(
            CurrentPage: CurrentPage,
            PageSize: PageSize,
            Sort: Sort,
            Name: Name,
            Color: Color,
            MinWeight: MinWeight,
            MaxWeight: MaxWeight,
            MinHeight: MinHeight,
            MaxHeight: MaxHeight,
            Breed_Id: Breed_Id,
            Species_Id: Species_Id,
            Health_Information: Health_Information,
            City: City,
            Street: Street,
            House_Number: House_Number,
            House_SubNumber: House_SubNumber,
            Appartment_Number: Appartment_Number,
            Owner_Phone: Owner_Phone,
            Is_Castrated: Is_Castrated,
            MinAge: MinAge,
            MaxAge: MaxAge,
            Is_Vacinated: Is_Vacinated,
            Status: Status,
            Volunteer_Id: VolunteerId);
    }
}
