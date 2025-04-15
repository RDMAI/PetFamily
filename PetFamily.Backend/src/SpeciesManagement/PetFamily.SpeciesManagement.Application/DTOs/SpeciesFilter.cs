using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.SpeciesManagement.Application.DTOs;

public record SpeciesFilter(string? SpeciesName = null, BreedId? BreedId = null, string? BreedName = null);
