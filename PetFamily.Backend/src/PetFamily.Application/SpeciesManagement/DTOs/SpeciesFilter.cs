using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.DTOs;

public record SpeciesFilter(string? SpeciesName = null, BreedId? BreedId = null, string? BreedName = null);
