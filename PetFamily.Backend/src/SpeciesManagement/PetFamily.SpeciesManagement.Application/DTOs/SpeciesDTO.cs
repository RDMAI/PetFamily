﻿namespace PetFamily.SpeciesManagement.Application.DTOs;

public class SpeciesDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    private ICollection<BreedDTO> Breeds { get; init; } = [];
}
