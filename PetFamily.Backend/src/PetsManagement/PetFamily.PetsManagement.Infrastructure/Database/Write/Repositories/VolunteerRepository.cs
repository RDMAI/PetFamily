﻿using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.PetsManagement.Domain.Entities;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Infrastructure.Database.Write.Repositories;

public class VolunteerRepository : IVolunteerAggregateRepository
{
    private readonly PetsWriteDBContext _context;

    public VolunteerRepository(PetsWriteDBContext context)
    {
        _context = context;
    }

    public async Task<Result<Volunteer, ErrorList>> GetByIdAsync(
        VolunteerId Id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Volunteers  //.Include(v => v.Pets) autoincluded
            .FirstOrDefaultAsync(v => v.Id == Id, cancellationToken);

        if (entity is null)
            return ErrorHelper.General.NotFound().ToErrorList();

        return entity;
    }

    public async Task<UnitResult<ErrorList>> IsEmailNotExistAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Volunteers
            .FirstOrDefaultAsync(v => v.Email == email, cancellationToken);

        if (entity is not null)
            return ErrorHelper.General.AlreadyExist("Email").ToErrorList();

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> IsPhoneNotExistAsync(
        Phone phone,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Volunteers
            .FirstOrDefaultAsync(v => v.Phone == phone, cancellationToken);

        if (entity is not null)
            return ErrorHelper.General.AlreadyExist("Phone").ToErrorList();

        return UnitResult.Success<ErrorList>();
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
        // Not needed because the changes is tracked
        //var entry = _context.Volunteers.Update(entity);

        await _context.SaveChangesAsync(cancellationToken);

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

    public async Task<Result<VolunteerId, ErrorList>> HardDeleteAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default)
    {
        _context.Volunteers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<Result<VolunteerId, ErrorList>> SoftDeleteAsync(
        Volunteer entity,
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
