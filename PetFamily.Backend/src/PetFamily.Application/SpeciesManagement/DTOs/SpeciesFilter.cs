using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.DTOs;

public record SpeciesFilter(string? SpeciesName = null, BreedId? BreedId = null, string? BreedName = null);
