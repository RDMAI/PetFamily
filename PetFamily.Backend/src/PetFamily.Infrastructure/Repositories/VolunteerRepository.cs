using CSharpFunctionalExtensions;
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

    public async Task<Result<Volunteer, Error>> GetByIdAsync(Guid Id)
    {
        var entity = await _context.Volunteers.FindAsync(Id);
        if (entity is null) return ErrorHelper.General.NotFound(); 

        return entity;
    }

    public async Task<Result<Guid, Error>> CreateAsync(Volunteer entity)
    {
        var entry = await _context.Volunteers.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entry.Entity.Id.Value;
    }
}
