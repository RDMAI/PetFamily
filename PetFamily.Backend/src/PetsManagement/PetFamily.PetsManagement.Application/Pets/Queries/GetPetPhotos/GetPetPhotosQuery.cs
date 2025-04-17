using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;

public record GetPetPhotosQuery(
    Guid VolunteerId,
    Guid PetId) : IQuery;
