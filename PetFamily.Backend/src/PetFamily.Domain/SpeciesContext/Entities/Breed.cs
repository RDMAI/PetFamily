using CSharpFunctionalExtensions;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.SpeciesContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PetFamily.Domain.SpeciesContext.Entities
{
    public class Breed : Entity<BreedId>
    {
        public string Name { get; private set; }

        private Breed(BreedId id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Breed> Create(BreedId id, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Breed>("Breed name cannot be empty");

            return Result.Success(new Breed(id, name));
        }
    }
}
