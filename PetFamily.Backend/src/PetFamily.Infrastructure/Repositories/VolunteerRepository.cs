using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository : IVolunteerRepository
{
    private readonly ApplicationDBContext _context;

    public VolunteerRepository(
        ApplicationDBContext context)
    {
        _context = context;
    }
    public async Task<Result<Volunteer, ErrorList>> GetByIdAsync(
        VolunteerId Id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Volunteers  //.Include(v => v.Pets) autoincluded
            .FirstOrDefaultAsync(v => v.Id == Id);

        //var entries = _context.ChangeTracker.Entries<Volunteer>();

        if (entity is null)
            return ErrorHelper.General.NotFound().ToErrorList();

        return entity;
    }

    public async Task<Result<VolunteerId, ErrorList>> CreateAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.Volunteers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity.Id;
    }

    public async Task<Result<VolunteerId, ErrorList>> UpdateAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default)
    {
        //var entries = _context.ChangeTracker.Entries<Volunteer>();

        // Not needed because the changes is tracked
        //var entry = _context.Volunteers.Update(entity);

        //var entriesAfterUpdate = _context.ChangeTracker.Entries<Volunteer>();

        await _context.SaveChangesAsync(cancellationToken);

        //var entriesAfterSave = _context.ChangeTracker.Entries<Volunteer>();

        return entity.Id;
    }

    public async Task<Result<IEnumerable<Volunteer>, ErrorList>> GetAsync(
        VolunteerFilter filter,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Volunteer> entities = _context.Volunteers.Include(v => v.Pets);

        // Filtering
        if (filter.FullName != null) entities = entities.Where(d =>
            d.FullName.FirstName == filter.FullName.FirstName &&
            d.FullName.LastName == filter.FullName.LastName &&
            d.FullName.FatherName == filter.FullName.FatherName);
        if (filter.Phone != null) entities = entities.Where(d => d.Phone == filter.Phone);
        if (filter.Email != null) entities = entities.Where(d => d.Email == filter.Email);

        // Pagination
        entities = entities.Skip(0).Take(100);  // add proper pagination later !!!!!

        return await entities.ToListAsync(cancellationToken);
    }
}
