﻿using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Species : Entity<SpeciesId>
{
    // EF Core
    private Species() { }
    public Species(SpeciesId id, SpeciesName name, List<Breed> breeds)
    {
        Id = id;
        Name = name;
        _breeds = breeds;
    }
    public SpeciesName Name { get; private set; }
    public IReadOnlyList<Breed> Breeds => _breeds;
    private List<Breed> _breeds = [];

    public Result<Species, Error> AddBreed(Breed breed)
    {
        if (breed is null)
            return ErrorHelper.General.ValueIsNullOrEmpty("Breed");

        _breeds.Add(breed);
        return this;
    }
}
