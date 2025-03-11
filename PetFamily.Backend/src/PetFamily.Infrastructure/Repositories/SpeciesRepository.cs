using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly ApplicationDBContext _context;

    public SpeciesRepository(IDbContextFactory<ApplicationDBContext> dbFactory)
    {
        _context = dbFactory.CreateDbContext();
    }

    public async Task<Result<IEnumerable<Species>, ErrorList>> GetAsync(
        SpeciesFilter filter,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
