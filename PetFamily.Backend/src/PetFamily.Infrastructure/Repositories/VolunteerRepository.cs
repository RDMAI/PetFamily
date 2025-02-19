using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers.Filters;
using PetFamily.Application.Volunteers.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteerRepository(ApplicationDBContext context) : IVolunteerRepository
{
    private readonly ApplicationDBContext _context = context;

    public async Task<Result<Volunteer, Error>> GetByIdAsync(
        Guid Id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Volunteers.FindAsync(Id, cancellationToken);
        if (entity is null) return ErrorHelper.General.NotFound(); 

        return entity;
    }

    public async Task<Result<Guid, Error>> CreateAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default)
    {
        var entry = await _context.Volunteers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity.Id.Value;
    }

    public async Task<Result<IEnumerable<Volunteer>, Error>> GetAsync(
        VolunteerFilter filter,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Volunteer> entities = _context.Volunteers;

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
