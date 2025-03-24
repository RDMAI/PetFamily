using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.Entities;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Infrastructure.DataBaseAccess.Write.Repositories;

public class SpeciesRepository : ISpeciesAggregateRepository
{
    private readonly WriteDBContext _context;

    public SpeciesRepository(WriteDBContext context)
    {
        _context = context;
    }

    public async Task<Result<Species, ErrorList>> GetByBreedIdAsync(
        BreedId breedId,
        CancellationToken cancellationToken = default)
    {
        var entity = await (from s in _context.Species  //.Include(v => v.Breeds) autoincluded
                            where s.Breeds.Any(b => b.Id == breedId)
                            select s)
                            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return ErrorHelper.General.NotFound().ToErrorList();

        return entity;
    }

    public async Task<Result<Species, ErrorList>> GetByIdAsync(
        SpeciesId Id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Species  //.Include(v => v.Breeds) autoincluded
            .FirstOrDefaultAsync(v => v.Id == Id, cancellationToken);

        if (entity is null)
            return ErrorHelper.General.NotFound().ToErrorList();

        return entity;
    }
}
