using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Application.Interfaces;
using PetFamily.SpeciesManagement.Domain.Entities;

namespace PetFamily.SpeciesManagement.Infrastructure.Database.Write.Repositories;

public class SpeciesRepository : ISpeciesAggregateRepository
{
    private readonly SpeciesWriteDBContext _context;

    public SpeciesRepository(SpeciesWriteDBContext context)
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

    public async Task<Result<SpeciesId, ErrorList>> HardDeleteAsync(
        Species entity,
        CancellationToken cancellationToken = default)
    {
        _context.Species.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<Result<SpeciesId, ErrorList>> UpdateAsync(Species entity, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
