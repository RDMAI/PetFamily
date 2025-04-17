using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreedById;

public record GetBreedByIdQuery(Guid BreedId) : IQuery;
