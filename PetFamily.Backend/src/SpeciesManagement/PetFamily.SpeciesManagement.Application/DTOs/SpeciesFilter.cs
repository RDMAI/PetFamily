using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.SpeciesManagement.Application.DTOs;

public record SpeciesFilter(string? SpeciesName = null, BreedId? BreedId = null, string? BreedName = null);
