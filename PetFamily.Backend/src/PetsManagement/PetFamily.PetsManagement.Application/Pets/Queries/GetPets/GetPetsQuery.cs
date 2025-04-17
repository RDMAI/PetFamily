using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPets;

public record GetPetsQuery(
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
    int? Status = null,
    Guid? Volunteer_Id = null
    ) : IQuery;
